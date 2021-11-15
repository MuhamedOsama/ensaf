using Ensaf.Domain.Enums;
using Ensaf.Domain.Models;
using Ensaf.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Resources
{
    public class CreateUserRequest
    {
       

        public string Name { get; set; }
        
        public RegisterUserTypes UserType { get; set; }


        public IEnumerable<string> Roles { get; set; }

       
        public string Email { get; set; }

      
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
