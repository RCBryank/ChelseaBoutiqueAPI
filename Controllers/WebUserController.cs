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


    }
}
