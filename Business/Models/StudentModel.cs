using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace Business.Models
{
    public class StudentModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [DisplayName("Student Name")]
        public string StudentName { get; set; }

        [Required]
        [StringLength(8)]
        public string Password { get; set; }

        [DisplayName("Active Student")]
        public bool IsActive { get; set; }

        [DisplayName("Grade")]
        public int GradeId { get; set; }

        public string GradeNameOutput { get; set; }

        [DisplayName("Active Student")]
        public string IsActiveOutput { get; set; }



    }
}
