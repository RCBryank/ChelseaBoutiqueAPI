using Chelsea_Boutique.Models;
using Chelsea_Boutique.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Supabase.Postgrest.Exceptions;

namespace Chelsea_Boutique.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IConfiguration _configuration, TokenProvider provider) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult> login(SignInCredentials credentials)
        {
            var url = _configuration.GetValue<string>("SupabaseAPI:URL");
            var key = _configuration.GetValue<string>("SupabaseAPI:Key");

            var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            try
            {
                var result = await supabase.From<WebUser>().Select(x => new object[] { x.ID, x.Name, x.WebUserPasswordSalt }).Where(x => x.Email == credentials.Email).Get();

                if (result.Model == null)
                    return Unauthorized(result);

                string hashedpassword = HashPasswordService.getHash(credentials.Password, result.Model.WebUserPasswordSalt);
                result = await supabase.From<WebUser>().Select(x => new object[] { x.ID, x.Name, x.Email, x.LastName }).Where(x => x.WebUserPassword == hashedpassword && x.ID == result.Model.ID).Get();

                if (result.Model == null)
                    return Unauthorized(result);

                var token = provider.Create(result.Model);


                var context = HttpContext;

                context?.Response.Cookies.Append("accessToken", token,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    }
                    );
                /* */

                WebUser _user = result.Model;

                return Ok(JsonConvert.SerializeObject(new WebUserProfileAuthenticatedInfo(_user.ID, _user.Email, _user.Name, _user.LastName, null)));
            }
            catch (Exception ex)
            {
            }

            string _h = credentials.Email;

            return new AcceptedAtActionResult("login", "auth", null, null);
        }

        [HttpPost("signup")]
        public async Task<ActionResult<TableTest>> SignUp(WebUser webuser)
        {
            var url = _configuration.GetValue<string>("SupabaseAPI:URL");
            var key = _configuration.GetValue<string>("SupabaseAPI:Key");

            var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            webuser.CreatedAt = DateTime.Now;
            webuser.ID_WebUserRol = 3;
            webuser.WebUserPasswordSalt = HashPasswordService.getSalt();
            webuser.WebUserPassword = HashPasswordService.getHash(webuser.WebUserPassword, webuser.WebUserPasswordSalt);

            try
            {
                var _result = await supabase.From<WebUser>().Insert(webuser, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });
                return CreatedAtAction("signup", JsonConvert.SerializeObject(_result), JsonConvert.SerializeObject(_result));
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine(ex.Reason);
                return BadRequest(ex.Content);
            }
        }
    }

    public class WebUserProfileAuthenticatedInfo
    {
        public WebUserProfileAuthenticatedInfo(int _id, string _email, string _name, string _lastName, string? _profilePhoto)
        {
            ID = _id;
            Email = _email;
            Name = _name;
            LastName = _lastName;
            ProfilePhoto = _profilePhoto;
        }

        public int ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string? ProfilePhoto { get; set; }
    }

    public class SignInCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public SignInCredentials()
        {
            Email = "";
            Password = "";
        }
    }
}
