using System.ComponentModel.DataAnnotations;

namespace Exam.Models.ViewModels
{
    public class StudentVM
    {
        public StudentVM()
        {
            this.CourseList = new List<int>();
        }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = default!;
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = default!;
        [Display(Name = "Image")]
        public IFormFile? ImagePath { get; set; }
        public string? Image { get; set; }
        public bool StudentStatus { get; set; }
        public List<int> CourseList { get; set; }
    }
}
