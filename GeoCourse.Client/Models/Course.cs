using System.Collections.Generic;

namespace GeoCourse.Client.Models
{
	public class Course
	{
		public int CourseId { get; set; }

		public string Title { get; set; }

		public string PicturePath { get; set; }

		public string Description { get; set; }

		public string Duration { get; set; }

		public int MaxPoints { get; set; }

		public int RequiredPoints { get; set; }

		public virtual ICollection<Test> Tests { get; set; }

		public virtual ICollection<UserCourse> UserCourses { get; set; }
	}
}