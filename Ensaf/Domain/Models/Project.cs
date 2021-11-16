using Ensaf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }
        public ProjectPackages ProjectPackage { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
