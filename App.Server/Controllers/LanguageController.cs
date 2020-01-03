using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Components.Resources;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/languages")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        [HttpGet("{language}/resources")]
        public ActionResult GetLayout(string language,string typeName)
        {
            var type = Type.GetType(typeName);
            var resourceManager = new ResourceManager(type);
            var data = resourceManager.ToDictionary(language);
            return new JsonResult(data);
        }
    }

    public static class ResourceManagerExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ResourceManager resourceManager, string locale)
        {
            var culture = new System.Globalization.CultureInfo(locale);
            var data = resourceManager.GetResourceSet(culture, true, true)
                .GetEnumerator();
            var res = new Dictionary<string, string>();
            while (data.MoveNext())
            {
                if (data.Key != null)
                {
                    res.Add(data.Key.ToString(), data.Value.ToString());
                }
            }
            return res;
        }
    }
}
