using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Chelsea_Boutique.Models
{
    [Table("tabletest")]
    public class TableTest : BaseModel
    {
        [PrimaryKey("ID")]
        public int ID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Value")]
        public int Value { get; set; }
    }
}
