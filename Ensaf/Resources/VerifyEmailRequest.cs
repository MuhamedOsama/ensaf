﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Resources
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
