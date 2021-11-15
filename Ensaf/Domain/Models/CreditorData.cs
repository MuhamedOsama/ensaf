using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class CreditorData
    {
        public int Id { get; set; }
        public string CreditorNameArabic { get; set; }
        public DateTime WorkStartDate { get; set; }
        public DateTime WorkEndDate { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        // if saudi
        public string IdNumber { get; set; }
        // if not saudi
        public string PassportNumber { get; set; }
        // all organization, maybe we're going to use an api ?
        // ????
        public string Organization { get; set; }
        //If Nationality != Saudi &&  Organization == inside Saudi Arabia
        public string IqamaId { get; set; }
        //(contain 6 blocks) (building number 0 Street name- neighborhood- City – postal code – Extra number) as per the image on jira
        public string NationalAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

    }
}
