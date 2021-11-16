using Ensaf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public User User { get; set; }
        public CreditorType CreditorType { get; set; }
        public ClaimType ClaimType { get; set; }
        public AdjectiveType AdjectiveType { get; set; }
        public double ValueInSaudiRyal { get; set; }
        public string Reason { get; set; }
        public DateTime OpeningDeed { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public double AmountPaid { get; set; }
        public DateTime DueDate { get; set; }
        public bool HasGuarantees { get; set; }
        // URL FOR THE ATTACHMENT
        public string GuaranteesDocument { get; set; }
        public string MaturityBond { get; set; }
        public string ApplicantName { get; set; }
        public int AgencyNumber { get; set; }
        public int IdentificationNumber { get; set; }
        // URL FOR THE ATTACHMENT
        public string AdjectiveDocument { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool AcceptDeclarationForm { get; set; }
        public CreditorData CreditorData { get; set; }
        public DebitData DebitData { get; set; }
        public BankInformation BankInformation { get; set; }
        public AdditionalInformation AdditionalInformation { get; set; }
        public ClaimAttachedFiles ClaimAttachedFiles { get; set; }
        public Project Project { get; set; }


    }
}
