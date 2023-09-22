using System;
using System.Collections.Generic;

namespace Exam.Models
{
    public partial class Enroll
    {
        public int EnrollId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Student? Student { get; set; }
    }
}
