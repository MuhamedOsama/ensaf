using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class BankInformation
    {
        public int Id { get; set; }
        public string IBAN { get; set; }
        // 
        public string BankNameArabic { get; set; }
        // anything except israel
        public string Country { get; set; }
        public string Nationality { get; set; }
        // if nationality is not saudi
        public string BankNameEnglish { get; set; }
        // URL FOR THE ATTACHMENT
        public string ProofOfAccountNumberDocument { get; set; }
    }
}
