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


			return View("Test");
		}
    }
}