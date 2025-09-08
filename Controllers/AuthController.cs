using Chelsea_Boutique.Models;
using Chelsea_Boutique.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Supabase.Postgrest.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Xml.Linq;

namespace Chelsea_Boutique.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IConfiguration _configuration, TokenProvider provider) : ControllerBase
    {
        private string varurl = _configuration.GetValue<string>("SupabaseAPI:URL");
        private string varkey = _configuration.GetValue<string>("SupabaseAPI:Key");

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

        [HttpPost("isAuthenticated")]
        [Authorize]
        public async Task<ActionResult> isAuthenticated([FromBody] int idwebuser)
        {
            var supabase = new Supabase.Client(varurl, varkey, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            try
            {
                var result = await supabase.From<WebUser>().Select(x => new object[] { x.ID_WebUserRol }).Where(x => x.ID == idwebuser /* int.Parse(idwebuser)*/).Get();

                if (result.Model == null)
                    return Ok(-1);

                return Ok(result.Model.ID_WebUserRol);
            }
            catch (PostgrestException ex)
            {

            }
            return Ok(-1);
        }

        [HttpGet("getinfo")]
        [Authorize]
        public async Task<ActionResult> GetInfo()
        {
            var supabase = new Supabase.Client(varurl, varkey, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            try
            {
                var vuserId = User.FindFirst("sub")?.Value;
                int userId;
                int.TryParse(vuserId, out userId);
                var result = await supabase.From<WebUser>().Select(x => new object[] { x.Email, x.Name, x.MiddleName, x.LastName, x.Address, x.PostalCode, x.City, x.Country, x.PhoneNumber, x.PhoneNumber2, x.DateofBirth }).Where(x => x.ID == userId).Get();

                if (result == null || result.Model == null)
                    return NotFound();

                var Model = result.Model;

                WebUserProfileExtendedDetailsInfo _info = new WebUserProfileExtendedDetailsInfo(Model.Email, Model.Name, Model.MiddleName, Model.LastName, Model.Address, Model.PostalCode,
                    Model.City, Model.Country, Model.PhoneNumber, Model.PhoneNumber2,
                    Model.DateofBirth.HasValue ? Model.DateofBirth.ToString() : "",
                    null);

                return Ok(JsonConvert.SerializeObject(_info));
            }
            catch (PostgrestException ex)
            {

            }
            return NotFound();
        }

        [HttpPut("updateprofile")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(WebUserProfileExtendedDetailsInfo webuser)
        {
            var supabase = new Supabase.Client(varurl, varkey, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            try
            {
                var vuserId = User.FindFirst("sub")?.Value;
                int userId;
                int.TryParse(vuserId, out userId);

                var result = await supabase.From<WebUser>().Where(x => x.ID == userId).Set(x => x.Name, webuser.Name)
                    .Set(x => x.MiddleName, webuser.MiddleName)
                    .Set(x => x.LastName, webuser.LastName)
                    .Set(x => x.Address, webuser.Address)
                    .Set(x => x.PostalCode, webuser.PostalCode)
                    .Set(x => x.City, webuser.City)
                    .Set(x => x.Country, webuser.Country)
                    .Set(x => x.PhoneNumber, webuser.PhoneNumber)
                    .Set(x => x.PhoneNumber2, webuser.PhoneNumber2)
                    .Set(x => x.DateofBirth, webuser.DateofBirth == "" ? null : DateTime.Parse(webuser.DateofBirth))
                    .Set(x => x.UpdatedAt, DateTime.Now)
                    .Update();

                return Ok(JsonConvert.SerializeObject(webuser));
            }
            catch (PostgrestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updatepassword")]
        [Authorize]
        public async Task<ActionResult> UpdatePassword([FromForm] string newwebuserpassword)
        {
            var supabase = new Supabase.Client(varurl, varkey, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            try
            {
                var vuserId = User.FindFirst("sub")?.Value;
                int userId;
                int.TryParse(vuserId, out userId);

                string webuserpasswordsalt = HashPasswordService.getSalt();
                string generatedwebuserpassword = HashPasswordService.getHash(newwebuserpassword, webuserpasswordsalt);

                var result = await supabase.From<WebUser>().Where(x => x.ID == userId).Set(x => x.WebUserPassword, null)
                    .Set(x => x.WebUserPasswordSalt, webuserpasswordsalt)
                    .Update();

                return Ok();
            }
            catch (PostgrestException ex)
            {
                return BadRequest(ex.Response);
            }
        }
    }

    public class WebUserProfileExtendedDetailsInfo
    {
        public WebUserProfileExtendedDetailsInfo(string Email, string Name, string MiddleName, string LastName, string Address, string PostalCode, string City, string Country, string PhoneNumber, string PhoneNumber2, string DateofBirth, string? ProfilePhoto)
        {
            this.Email = Email;
            this.Name = Name;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Address = Address;
            this.PostalCode = PostalCode;
            this.City = City;
            this.Country = Country;
            this.PhoneNumber = PhoneNumber;
            this.PhoneNumber2 = PhoneNumber2;
            this.DateofBirth = DateofBirth;
            this.ProfilePhoto = ProfilePhoto;
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string DateofBirth { get; set; }
        public string? ProfilePhoto { get; set; }
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
