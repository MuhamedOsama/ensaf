using Ensaf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IdentificationTypes IdType { get; set; }

#nullable enable
        public string? IdNumber { get; set; }
        public string? Iqama { get; set; }
        public string? PassportNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public string? OfficeName { get; set; }
        public string? OfficeAddress { get; set; }
#nullable disable
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool AcceptTerms { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
        public string VerificationToken { get; set; }
        public DateTime? Verified { get; set; }
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
