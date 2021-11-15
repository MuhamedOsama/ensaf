using AutoMapper;
using Ensaf.Domain.Enums;
using Ensaf.Domain.Models;
using Ensaf.Domain.Services;
using Ensaf.Helpers;
using Ensaf.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ensaf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (!Account.OwnsToken(token) && Account.UserRoles.Any(r => r.Role.Name != nameof(Roles.SysAdmin)))
                return Unauthorized(new { message = "Unauthorized" });

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model, Request.Headers["origin"]);
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            _userService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
        {
            _userService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        [Authorize(Roles.SysAdmin)]
        [HttpGet]
        public ActionResult<IEnumerable<UserResponse>> GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public ActionResult<UserResponse> GetById(int id)
        {
            // users can get their own user and admins can get any user
            if (id != Account.Id && !Account.UserRoles.Any(r => r.Role.Name == nameof(Roles.SysAdmin)))
                return Unauthorized(new { message = "Unauthorized" });

            var user = _userService.GetById(id);
            return Ok(user);
        }

        [Authorize(Roles.SysAdmin)]
        [HttpPost]
        public ActionResult<UserResponse> Create(CreateUserRequest model)
        {
            var user = _userService.Create(model);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public ActionResult<UserResponse> Update(int id, UpdateUserRequest model)
        {
            // users can update their own user and admins can update any user
            if (id != Account.Id && !Account.UserRoles.Any(r => r.Role.Name== nameof(Roles.SysAdmin)))
                return Unauthorized(new { message = "Unauthorized" });

            // only admins can update role
            if (Account.UserRoles.Any(r => r.Role.Name != nameof(Roles.SysAdmin)))
                model.Roles = null;

            var user = _userService.Update(id, model);
            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            // users can delete their own user and admins can delete any user
            if (id != Account.Id && !Account.UserRoles.Any(r=> r.Role.Name == nameof(Roles.SysAdmin)))
                return Unauthorized(new { message = "Unauthorized" });

            _userService.Delete(id);
            return Ok(new { message = "User deleted successfully" });
        }

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
