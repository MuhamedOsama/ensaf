using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Resources
{
    public class UpdateUserRequest
    {
        private string _password;
        private string _confirmPassword;
        private IEnumerable<string> _roles;
        private string _email;

        public string Name {get; set; }
        


        public IEnumerable<string> Roles
        {
            get => _roles;
            set => _roles = replaceEmptyWithNullList(value);
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
        private IEnumerable<string> replaceEmptyWithNullList(IEnumerable <string> values)
        {
            // replace empty string with null to make field optional
            return !values.Any() || values == null ? null : values;
        }
        
    }
}
