using System.Collections.Generic;
using System.Linq;

namespace GeoCourse.Client.ViewModels
{
	public class TestViewModel
	{
		public int TestId { get; set; }

		public IList<QuestionViewModel> Questions { get; set; }
	}
}