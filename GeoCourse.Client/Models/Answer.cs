namespace GeoCourse.Client.Models
{
	public class Answer
	{
		public int AnswerId { get; set; }

		public string Title { get; set; }

		public bool IsCorrect { get; set; }

		public int? QuestionId { get; set; }

		public virtual Question Question { get; set; }
	}
}