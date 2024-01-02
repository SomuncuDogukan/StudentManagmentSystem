#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class CourseModel
    {



        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(30)]
        public string Course_Title { get; set; }

        public string Course_Content { get; set; }




        [DisplayName("Student Count")]
        public int StudentCountOutput { get; set; }

        [DisplayName("Students")]
        //[Required(ErrorMessage = "{0} must be selected!")] // if users must be selected, uncomment this line
        public List<int> StudentIdsInput { get; set; }


        [DisplayName("Students")]
        public string StudentNamesOutput { get; set; }






    }
}
