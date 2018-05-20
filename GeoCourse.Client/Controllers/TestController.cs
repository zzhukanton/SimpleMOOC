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
		// 7 questions
		public const int QUESTION_COUNT_PER_TEST = 5;
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
			var test = _context.Tests.Find(id);
			ViewBag.Title = test.Chapter;

			// probably simplify later
			var courseId = test.CourseId;
			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourse = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId && uc.UserId == userId);
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == test.TestId && tr.UserCourseId == userCourse.UserCourseId);

			if (testResult.CurrentTryCount >= testResult.MaxTryCount)
			{
				ViewBag.Blocked = true;
				return View();
			}
			else
			{
				var questions = _context.Questions
				.Where(q => q.TestId == id)
				.Select(q => new QuestionViewModel()
				{
					QuestionId = q.QuestionId,
					Title = q.Text,
					Answers = _context.Answers
						.Where(a => a.QuestionId == q.QuestionId)
						.Select(a => new AnswerViewModel()
						{
							AnswerId = a.AnswerId,
							Title = a.Title
						})
						.AsEnumerable()
				}).Take(5).ToList();
				var model = new TestViewModel()
				{
					TestId = id,
					Questions = questions
				};
				ViewBag.Blocked = false;

				return View(model);
			}
			
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
			ViewBag.TestId = model.TestId;

			var courseId = _context.Tests.Find(model.TestId).CourseId;
			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourse = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId && uc.UserId == userId);
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == model.TestId && tr.UserCourseId == userCourse.UserCourseId);

			// update test result
			testResult.CurrentTryCount++;
			if (testResult.PointCount < correctAnswerCount)
			{
				userCourse.CurrentPoints -= testResult.PointCount;
				testResult.PointCount = correctAnswerCount;
				userCourse.CurrentPoints += correctAnswerCount;
			} 
			_context.Entry(testResult).State = System.Data.Entity.EntityState.Modified;
			_context.Entry(userCourse).State = System.Data.Entity.EntityState.Modified;

			_context.SaveChanges();

			ViewBag.MaxTryReached = testResult.CurrentTryCount >= testResult.MaxTryCount;
			ViewBag.CourseId = courseId;

			return View();
		}

		[HttpGet]
		public ActionResult FinalTest(int id)
		{
			var userCourse = _context.UserCourses.Find(_context.TestResults.Find(id).UserCourseId);
			ViewBag.CourseTitle = _context.Courses.Find(userCourse.CourseId).Title;

			var questions = _context.Tests.Where(t => t.CourseId == userCourse.CourseId)
				.SelectMany(t => _context.Questions.Where(q => q.TestId == t.TestId).Take(3).ToList()).ToList();

			var testViewModel = new TestViewModel()
			{
				Questions = questions.Select(q => new QuestionViewModel()
				{
					Answers = q.Answers.Select(a => new AnswerViewModel()
					{
						AnswerId = a.AnswerId,
						Title = a.Title
					}),
					QuestionId = q.QuestionId,
					Title = q.Text
				}).ToList()
			};

			return View(testViewModel);
		}

		[HttpPost]
		public ActionResult VerifyFinalTest(TestViewModel model)
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

			var anyQuestionId = _context.Answers.Find(selectedAnswerIds.FirstOrDefault()).QuestionId;

			var courseId = _context.Tests.Find(_context.Questions.Find(anyQuestionId).TestId).CourseId;
			var course = _context.Courses.Find(courseId);
			ViewBag.CourseTitle = course.Title;

			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourse = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == course.CourseId && uc.UserId == userId);
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == null && tr.UserCourseId == userCourse.UserCourseId);

			testResult.PointCount = correctAnswerCount;
			_context.Entry(testResult).State = System.Data.Entity.EntityState.Modified;
			userCourse.DateCompleted = DateTime.Now;

			_context.Entry(userCourse).State = System.Data.Entity.EntityState.Modified;
			_context.SaveChanges();

			ViewBag.Correct = correctAnswerCount;
			var all = selectedAnswerIds.Count();
			ViewBag.All = all;
			var testResults = _context.TestResults.Where(tr => tr.UserCourseId == userCourse.UserCourseId && tr.TestId != null).ToList();
			var testResultsScore = (double)testResults.Sum(tr => tr.PointCount) / (testResults.Count * 5);
			ViewBag.TestResults = testResults;
			ViewBag.Tests = _context.Tests.Where(t => t.CourseId == course.CourseId).ToList();
			ViewBag.FinalPercentage = (0.6 * correctAnswerCount / all + 0.4 * testResultsScore) * 100;

			return View("FinalTestResult");
		}
	}
}