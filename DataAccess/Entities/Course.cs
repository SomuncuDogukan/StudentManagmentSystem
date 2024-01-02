#nullable disable 
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities;

public class Course
{
    
    public int Id { get; set; } 

    [Required]
    [StringLength(30)]
    public string Course_Title { get; set; } 

    public string Course_Content { get; set; }

 
    public List<StudentCourse> StudentCourses { get; set; }
}
