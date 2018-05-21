using System;
using System.Collections.Generic;

namespace GeoCourse.Client.Models
{
	public class UserCourse
	{
		public int UserCourseId { get; set; }

		public Guid? UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		public int? CourseId { get; set; }

		public virtual Course Course { get; set; }

		public bool IsFinished { get; set; }

		public bool? IsCompleted { get; set; }

		public int? FinalCourseScore { get; set; }

		public int CurrentPoints { get; set; }

		public DateTime? DateCompleted { get; set; }

		public virtual ICollection<TestResult> TestResults { get; set; }
	}
}