using System.Collections.Generic;

namespace GeoCourse.Client.ViewModels
{
	public class QuestionViewModel
	{
		public int QuestionId { get; set; }

		public string Title { get; set; }

		public int? SelectedAnswer { get; set; }

		public IEnumerable<AnswerViewModel> Answers { get; set; }
	}
}