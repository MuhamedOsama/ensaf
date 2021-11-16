using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Extensions
{
    public static class RuleBuilderExtension
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 14)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Password Is Required.")
                .MinimumLength(8).WithMessage("Password Must Contain At Least 8 Letters")
                .Matches("[A-Z]").WithMessage("Password Must Contain Uppercase Letter.")
                .Matches("[a-z]").WithMessage("Password Must Contain Lowercase Letter.")
                .Matches("[0-9]").WithMessage("Password Must Contain At Least 1 Digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password Must Contain One Special Character.");
            return options;
        }

        public static IRuleBuilderOptions<T, string> IsPhoneNumber<T>(this IRuleBuilder<T, string> rule) => rule.Matches(@"^(1-)?\d{3}-\d{3}-\d{4}$").WithMessage("Invalid phone number");
    }
}
