using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Task2.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [StringLength(50)]
        [Column("user_username")]
        public string UserName { get; set; }

        [MaxLength(50)]
        [Column("user_hash_password")]
        public byte[] UserHashPassword { get; set; }

        [Column("role_id")]
        public byte RoleId { get; set; }
        public Role Role { get; set; }

        [StringLength(50)]
        [Column("user_email")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Column("create_at")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Column("update_at")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}
