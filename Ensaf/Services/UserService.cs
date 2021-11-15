using AutoMapper;
using Ensaf.Domain.Enums;
using Ensaf.Domain.Models;
using Ensaf.Domain.Services;
using Ensaf.Helpers;
using Ensaf.Persistence;
using Ensaf.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Ensaf.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(
            ApplicationDbContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings
            )
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = _context.Users.Include(r => r.UserRoles).ThenInclude(r => r.Role).SingleOrDefault(x => x.Email == model.Email);

            if (user == null || !user.IsVerified || !BC.Verify(model.Password, user.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            // save changes to db
            _context.Update(user);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, user) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);

            removeOldRefreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, user) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            // validate
            if (_context.Users.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent user enumeration
                // sendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new user object
            var user = _mapper.Map<User>(model);
            Role role = null;
            if(model.UserType == RegisterUserType.NormalUser)
            {
                role = _context.Roles.FirstOrDefault(r => r.Name == nameof(Roles.Normal));

            }else if (model.UserType == RegisterUserType.Commissioner)
            {
                role = _context.Roles.FirstOrDefault(r => r.Name == nameof(Roles.Commissioner));
            }
            else
            {
                throw new AppException("Invalid User Type.");
            }
            user.UserRoles.Add(new UserRole { Role = role, User = user });
            user.Created = DateTime.UtcNow;
            user.VerificationToken = randomTokenString();
            //remove later, should verify using email.
            user.Verified = DateTime.Now ;
            // hash password
            user.PasswordHash = BC.HashPassword(model.Password);

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();

            // send email
            //sendVerificationEmail(user, origin);
        }

        public void VerifyEmail(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.VerificationToken == token);

            if (user == null) throw new AppException("Verification failed");

            user.Verified = DateTime.UtcNow;
            user.VerificationToken = null;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (user == null) return;

            // create reset token that expires after 1 day
            user.ResetToken = randomTokenString();
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Users.Update(user);
            _context.SaveChanges();

            // send email
            sendPasswordResetEmail(user, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var user = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var user = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            user.PasswordHash = BC.HashPassword(model.Password);
            user.PasswordReset = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public IEnumerable<UserResponse> GetAll()
        {
            var users = _context.Users;
            return _mapper.Map<IList<UserResponse>>(users);
        }

        public UserResponse GetById(int id)
        {
            var user = getUser(id);
            return _mapper.Map<UserResponse>(user);
        }

        public UserResponse Create(CreateUserRequest model)
        {
            // validate
            if (_context.Users.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new user object
            var user = _mapper.Map<User>(model);
            var roles = _context.Roles.Where(r => model.Roles.Contains(r.Name)).ToList();
            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole { Role = role, User = user });
            }
            user.Created = DateTime.UtcNow;
            user.Verified = DateTime.UtcNow;

            // hash password
            user.PasswordHash = BC.HashPassword(model.Password);
            //remove later, should verify using email.
            user.Verified = DateTime.Now;
            // save user
            _context.Users.Add(user);
            _context.SaveChanges();

            return _mapper.Map<UserResponse>(user);
        }

        public UserResponse Update(int id, UpdateUserRequest model)
        {
            var user = getUser(id);

            // validate
            if (user.Email != model.Email && _context.Users.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = BC.HashPassword(model.Password);

            // copy model to user and save
            _mapper.Map(model, user);
            user.Updated = DateTime.UtcNow;
            _context.Users.Update(user);
            _context.SaveChanges();

            return _mapper.Map<UserResponse>(user);
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        private (RefreshToken, User) getRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null) throw new AppException("Invalid token");
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, user);
        }
        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[] { new System.Security.Claims.Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(900),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(User user)
        {
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        //private void sendVerificationEmail(User user, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //    {
        //        var verifyUrl = $"{origin}/user/verify-email?token={user.VerificationToken}";
        //        message = $@"<p>Please click the below link to verify your email address:</p>
        //                     <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        //    }
        //    else
        //    {
        //        message = $@"<p>Please use the below token to verify your email address with the <code>/users/verify-email</code> api route:</p>
        //                     <p><code>{user.VerificationToken}</code></p>";
        //    }

        //    _emailService.Send(
        //        to: user.Email,
        //        subject: "Sign-up Verification API - Verify Email",
        //        html: $@"<h4>Verify Email</h4>
        //                 <p>Thanks for registering!</p>
        //                 {message}"
        //    );
        //}

        //private void sendAlreadyRegisteredEmail(string email, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //        message = $@"<p>If you don't know your password please visit the <a href=""{origin}/user/forgot-password"">forgot password</a> page.</p>";
        //    else
        //        message = "<p>If you don't know your password you can reset it via the <code>/users/forgot-password</code> api route.</p>";

        //    _emailService.Send(
        //        to: email,
        //        subject: "Sign-up Verification API - Email Already Registered",
        //        html: $@"<h4>Email Already Registered</h4>
        //                 <p>Your email <strong>{email}</strong> is already registered.</p>
        //                 {message}"
        //    );
        //}

        private void sendPasswordResetEmail(User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/user/reset-password?token={user.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/users/reset-password</code> api route:</p>
                             <p><code>{user.ResetToken}</code></p>";
            }

            //_emailService.Send(
            //    to: user.Email,
            //    subject: "Sign-up Verification API - Reset Password",
            //    html: $@"<h4>Reset Password Email</h4>
            //             {message}"
            //);
        }
    }
}
