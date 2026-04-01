using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    [Table("designationTbl")]
    public class Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int designationId { get; set; }
        public int departmentId { get; set; }
        [Required, MaxLength(50)]
        public string designationName { get; set; } = string.Empty;
    }
}
