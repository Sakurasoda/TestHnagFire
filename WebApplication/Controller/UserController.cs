using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controller
{
    [Microsoft.AspNetCore.Components.Route("[controller]")]
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            // write use id 
            HttpContext.Session.Set("useId", Encoding.UTF8.GetBytes("tester"));
            
            return Redirect("/hangfire");
        }
    }
}