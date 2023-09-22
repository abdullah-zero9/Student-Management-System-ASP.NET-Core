using System;
using System.Collections.Generic;

namespace Exam.Models
{
    public partial class Course
    {
        public Course()
        {
            Enrolls = new HashSet<Enroll>();
        }

        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;

        public virtual ICollection<Enroll> Enrolls { get; set; }
    }
}
