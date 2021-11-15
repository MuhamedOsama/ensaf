using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Domain.Enums
{
    public enum FirstDetailTypes
    {
        Accountant,
        Engineer,
        Representative,
        Manager,
        Worker,
        Driver,
        Cooker,
        Administrative,
        Lawyer,
        Consultant,
        Guarantee,
        FalteringProject


//            If Relationship == Employee

//(Accountant-Engineer – representative- manager- Worker- Driver – Cooker– Administrative- Lawyer – Consultant)


//            If Relationship == Retired
//(Lawyer – Consultant)



//            If Relationship == beneficiary
//(a guarantee - faltering project)
    }
}
