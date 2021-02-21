using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ELearningView
    {
        public long StudentCourseRegistrationDetailId { get; set; }
        public long? StudentCourseRegistrationId { get; set; }
        public long CourseId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public int EContentTypeId { get; set; }
        public int? Views { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int LevelId { get; set; }
        public long PersonId { get; set; }
        public string ProgrammeName { get; set; }
        public int ProgrammeId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string VideoUrl { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ECourseId { get; set; }
        public string SetDate { get; set; }
        public string DueDate { get; set; }
        public long AssignmentId { get; set; }
        public bool HasSubmitted { get; set; }
    }
}
