using GeoCourse.Client.Models;
using GeoCourse.Client.ViewModels;
using Microsoft.AspNet.Identity;
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
		private const int QUESTION_COUNT_PER_TEST = 5;
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
			var failedQuestions = new List<string>();

			foreach (var answerId in selectedAnswerIds)
			{
				var answer = _context.Answers.Find(answerId);
				if (answer.IsCorrect)
					correctAnswerCount++;
				else
					failedQuestions.Add(_context.Questions.Find(answer.QuestionId).Text);
			}

			ViewBag.AnswerCount = QUESTION_COUNT_PER_TEST;
			ViewBag.CorrectAnswerCount = correctAnswerCount;
			ViewBag.Percent = ((double)correctAnswerCount / QUESTION_COUNT_PER_TEST) * 100;
			ViewBag.FailedQuestions = failedQuestions;

			var courseId = _context.Tests.Find(model.TestId).CourseId;
			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourse = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId && uc.UserId == userId);
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == model.TestId && tr.UserCourseId == userCourse.UserCourseId);

			// update test result
			testResult.CurrentTryCount++;
			if (testResult.PointCount < correctAnswerCount)
			{
				testResult.PointCount = correctAnswerCount;
			} 
			_context.Entry(testResult).State = System.Data.Entity.EntityState.Modified;
			userCourse.CurrentPoints += correctAnswerCount;
			_context.Entry(userCourse).State = System.Data.Entity.EntityState.Modified;

			_context.SaveChanges();

			return View();
		}
    }
}