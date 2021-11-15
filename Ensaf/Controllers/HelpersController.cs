using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HelpersController : ControllerBase
    {
        [HttpGet("GetAllNationalities")]
        public IActionResult GetNationalities()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            var cultureList = new List<string>();
            foreach (var c in cultures)
            {
                RegionInfo regionInfo = new RegionInfo(c.LCID);
                if (!(cultureList.Contains(regionInfo.EnglishName)) && regionInfo.EnglishName != "Israel")
                {
                    cultureList.Add(regionInfo.EnglishName);
                }

            }
            return Ok(cultureList);
        }
    }
}
