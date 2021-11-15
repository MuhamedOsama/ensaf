using Ensaf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class AdditionalInformation
    {
        public int Id { get; set; }
        // readonly text
        public string Category { get; set; } = "Individual";
        public RelationshipTypes RelationshipType { get; set; }
        // if relationship == employee
        public FirstDetailTypes FirstDetails { get; set; }
    }
}
