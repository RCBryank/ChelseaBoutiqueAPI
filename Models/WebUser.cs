using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;


namespace Chelsea_Boutique.Models
{
    [Table("webuser")]
    public class WebUser : BaseModel
    {
        /*public WebUser(int iD, string email, string password, string middleName, string lastName, DateTime dateofBirth, string address, string city, string country, string postalCode, string phoneNumber, string phoneNumber2, DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt)
        {
            //ID = iD;
            Email = email;
            WebUserPassword = password;
            MiddleName = middleName;
            LastName = lastName;
            /*
            DateofBirth = dateofBirth;
            Address = address;
            City = city;
            Country = country;
            PostalCode = postalCode;
            PhoneNumber = phoneNumber;
            PhoneNumber2 = phoneNumber2;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
        }*/

        [PrimaryKey("ID")]
        public int ID { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("WebUserPassword")]
        public string WebUserPassword { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("MiddleName")]
        public string MiddleName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("DateofBirth")]
        public DateTime? DateofBirth { get; set; }

        [Column("Address")]
        public string Address { get; set; }

        [Column("City")]
        public string City { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("PostalCode")]
        public string PostalCode { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("PhoneNumber2")]
        public string PhoneNumber2 { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt", ignoreOnInsert: true)]
        public DateTime? UpdatedAt { get; set; }

        [Column("DeletedAt", ignoreOnInsert: true)]
        public DateTime? DeletedAt { get; set; }

        [Column("ID_WebUserRol")]
        public int ID_WebUserRol { get; set; }

        [Column("WebUserPasswordSalt")]
        public string WebUserPasswordSalt { get; set; }
    }
}
