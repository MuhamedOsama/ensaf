﻿using Ensaf.Domain.Enums;

namespace Ensaf.Resources
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public IdentificationType IdType { get; set; }
        public string PassportNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string PhoneNumber { get; set; }

        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }
        public bool AcceptTerms { get; set; }

        public RegisterUserType UserType { get; set; }

        public string IdNumber { get; set; }

        public string Iqama { get; set; }

        public string Email { get; set; }


        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
