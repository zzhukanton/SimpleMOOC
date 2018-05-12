using System.Collections.Generic;

namespace GeoCourse.Client.Models
{
	public class Question
	{
		public int QuestionId { get; set; }

		public string Text { get; set; }

		public int Points { get; set; }

		public int? TestId { get; set; }

		public virtual Test Test { get; set; }

		public virtual ICollection<Answer> Answers { get; set; }
	}
}