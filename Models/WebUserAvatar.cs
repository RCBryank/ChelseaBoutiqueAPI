using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Chelsea_Boutique.Models
{
    [Table("webuseravatar")]
    public class WebUserAvatar : BaseModel
    {
        [PrimaryKey("id")]
        public int ID { get; set; }

        [Column("filename")]
        public string Filename { get; set; }

        [Column("publicpath")]
        public string PublicPath { get; set; }

        [Column("filesizekb")]
        public int FileSizeKb { get; set; }

        [Column("assetsignature")]
        public string AssetSignature { get; set; }

        [Column("id_webuser")]
        public int ID_WebUser { get; set; }
    }
}
