using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Grade
    {
        public int Id { get; set; } // this is called a property in C# which contains getters and setters 

        [Required]
        [StringLength(10, MinimumLength = 4)]
        public string Name { get; set; } // "String" class type can also be used, general usage "string" data type

        public List<Student> Students { get; set; }



    }
}
