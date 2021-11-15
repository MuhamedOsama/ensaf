using Ensaf.Domain.Enums;
using Ensaf.Resources;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => (int)x.IdType).NotEmpty().InclusiveBetween(1, 3);
            RuleFor(x => x.PassportNumber).NotEmpty().When(u => (int)u.IdType == (int)IdentificationTypes.Passport);
            RuleFor(x => x.IdNumber).NotEmpty().When(u => (int)u.IdType == (int)IdentificationTypes.KsA);
            RuleFor(x => x.Iqama).NotEmpty().When(u => (int)u.IdType == (int)IdentificationTypes.Iqama);
            RuleFor(x => (int)x.UserType).NotEmpty().InclusiveBetween(1, 2);
            RuleFor(x => x.LicenseNumber).NotEmpty().When(u => (int)u.UserType == (int)RegisterUserTypes.Commissioner);
            RuleFor(x => x.OfficeAddress).NotEmpty().When(u => (int)u.UserType == (int)RegisterUserTypes.Commissioner);
            RuleFor(x => x.OfficeName).NotEmpty().When(u => (int)u.UserType == (int)RegisterUserTypes.Commissioner);
            RuleFor(x => x.AcceptTerms).NotEmpty().Equal(true).WithMessage("Terms Must Be Accepted.");
        }
    }
}
