using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string StudentName { get; set; }

        [Required]
        [StringLength(8)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

       

        //has-a relationship
        public Grade Grade { get; set; }

        // tables one to many relationship 
        public int GradeId { get; set; }

        // many to many tables relationship
        public List<StudentCourse> StudentCourses { get; set; }








    }
}
