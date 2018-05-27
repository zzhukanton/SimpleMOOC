using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeoCourse.Client.ViewModels
{
	public class QuestionViewModel
	{
		public int QuestionId { get; set; }

		public string Title { get; set; }

		[Required(ErrorMessage = "Вы не выбрали вариант ответа")]
		public int? SelectedAnswer { get; set; }

		public IEnumerable<AnswerViewModel> Answers { get; set; }
	}
}