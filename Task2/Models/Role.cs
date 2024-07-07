using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task2.Models
{
    [Table("role")]
    public class Role
    {
        [Column("role_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte RoleId { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }
    }
}
