using GeoCourse.Client.Models;
using GeoCourse.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoCourse.Client.Controllers
{
	[Authorize]
    public class TestController : Controller
    {
		private const int QUESTION_COUNT_PER_TEST = 6;
		public ApplicationDbContext _context;

		public TestController()
		{
			_context = new ApplicationDbContext();
		}

        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public ActionResult Test(int id)
		{
			ViewBag.Title = _context.Tests.Find(id).Chapter;
			var questions = _context.Questions
				.Where(q => q.TestId == id)
				.Select(q => new QuestionViewModel() {
					QuestionId = q.QuestionId,
					Title = q.Text,
					Answers = _context.Answers
						.Where(a => a.QuestionId == q.QuestionId)
						.Select(a => new AnswerViewModel() {
							AnswerId = a.AnswerId,
							Title = a.Title
						})
						.AsEnumerable()
				}).ToList();
			var model = new TestViewModel()
			{
				TestId = id,
				Questions = questions
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult Verify(TestViewModel model)
		{
			var selectedAnswerIds = model.Questions.Select(q => q.SelectedAnswer);
			int correctAnswerCount = 0;
			foreach (var answerId in selectedAnswerIds)
			{
				var answer = _context.Answers.Find(answerId);
				if (answer.IsCorrect)
					correctAnswerCount++;
			}

			//var answers = _context.Answers.Where(a => model.Questions.Select(q => q.SelectedAnswer).Contains(a.AnswerId)).AsEnumerable();
			ViewBag.AnswerCount = QUESTION_COUNT_PER_TEST;
			ViewBag.CorrectAnswerCount = correctAnswerCount;



			return View();
		}
    }
}