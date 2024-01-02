#nullable disable

using DataAccess.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class GradeModel
    {
        public int Id { get; set; } // this is called a property in C# which contains getters and setters 

        [Required]
        [StringLength(10, MinimumLength = 4)]
        public string Name { get; set; } // "String" class type can also be used, general usage "string" data type

        #region Extra properties required for the views
        [DisplayName("Student Count")]
        public int StudentCountOutput { get; set; }
    }
}
#endregion