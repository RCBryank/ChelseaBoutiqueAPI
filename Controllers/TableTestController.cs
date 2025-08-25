using Chelsea_Boutique.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Chelsea_Boutique.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TableTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(Name = "GetTableTest")]
        public async Task<IEnumerable<TableTest>> Get()
        {
            var url = _configuration.GetValue<string>("SupabaseAPI:URL");
            var key = _configuration.GetValue<string>("SupabaseAPI:Key");

            var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            var response = await supabase.From<TableTest>().Get();
            var rows = response.Models;

            //var _result = await supabase.From<TableTest>().Insert(new TableTest { Value = 3 }, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });

            return rows;
        }

        [HttpPost]
        public async Task<ActionResult<TableTest>> Post(TableTest tabletest)
        {
            var url = _configuration.GetValue<string>("SupabaseAPI:URL");
            var key = _configuration.GetValue<string>("SupabaseAPI:Key");

            var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            await supabase.InitializeAsync();

            var _result = await supabase.From<TableTest>().Insert(tabletest, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });

            return CreatedAtAction("post", JsonConvert.SerializeObject(tabletest), JsonConvert.SerializeObject(_result.Model));
        }
    }
}
