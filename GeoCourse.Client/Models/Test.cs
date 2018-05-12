using System.Collections.Generic;

namespace GeoCourse.Client.Models
{
	public class Test
	{
		public int TestId { get; set; }

		public string Chapter { get; set; }

		public string DocumentPath { get; set; }

		public string Description { get; set; }

		public int? CourseId { get; set; }

		public virtual Course Course { get; set; }

		public virtual ICollection<Question> Questions { get; set; }
	}
}