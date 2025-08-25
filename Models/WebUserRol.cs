using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Chelsea_Boutique.Models
{
    public class WebUserRol : BaseModel
    {
        [PrimaryKey("ID")]
        public int ID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }
    }
}
