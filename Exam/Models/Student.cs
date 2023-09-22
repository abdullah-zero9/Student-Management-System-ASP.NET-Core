using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public partial class Student
    {
        public Student()
        {
            Enrolls = new HashSet<Enroll>();
        }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = null!;
        public string? Image { get; set; }
        public bool StudentStatus { get; set; }

        public virtual ICollection<Enroll> Enrolls { get; set; }


    }
}
