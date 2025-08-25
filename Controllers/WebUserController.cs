using Chelsea_Boutique.Models;
using Chelsea_Boutique.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase.Postgrest.Exceptions;

namespace Chelsea_Boutique.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WebUserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<TableTest>> Post(WebUser webuser)
        {
            var url = _configuration.GetValue<string>("SupabaseAPI:URL");
            var key = _configuration.GetValue<string>("SupabaseAPI:Key");

            var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            //var _result = await supabase.Auth.SignUp(webuser.Email, webuser.WebUserPassword);
            webuser.CreatedAt = DateTime.Now;//.ToString("yyyy-MM-dd");
            webuser.ID_WebUserRol = 3;
            webuser.WebUserPasswordSalt = HashPasswordService.getSalt();
            webuser.WebUserPassword = HashPasswordService.getHash(webuser.WebUserPassword, webuser.WebUserPasswordSalt);

            try
            {
                var _result = await supabase.From<WebUser>().Insert(webuser, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });
                return CreatedAtAction("post", JsonConvert.SerializeObject(_result), JsonConvert.SerializeObject(_result));
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine(ex.Reason);
                return BadRequest(ex.Content);
            }
        }
    }

    public interface IPasswordHasher<WebUser> where WebUser : class
    {
        string Hash(WebUser webuser, string password);

        PasswordVerificationResult VerifyHashedPassword(WebUser webuser, string password, string providedpassword);
    }
}
