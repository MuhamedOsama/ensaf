using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Resources
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
    }
}
