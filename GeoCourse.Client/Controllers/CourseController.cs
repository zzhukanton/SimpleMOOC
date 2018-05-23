using GeoCourse.Client.Models;
using GeoCourse.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;

namespace GeoCourse.Client.Controllers
{
    public class CourseController : Controller
    {
		private ApplicationDbContext _context;

		public CourseController()
		{
			_context = new ApplicationDbContext();
		}

        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

		public FileStreamResult LoadPdf()
		{
			//var path = 
			FileStream fs = new FileStream(@"E:\PVS\PVS\plc\Startup.pdf", FileMode.Open, FileAccess.Read);
			return File(fs, "application/pdf");
		}

		[HttpGet]
		public ActionResult Course(int id)
		{
			var course = _context.Courses.Find(id);
			ViewBag.Course = course;

			if (Request.IsAuthenticated)
			{
				ViewBag.CourseMap = _context.Tests.Where(t => t.CourseId == course.CourseId).AsQueryable().ToList();
				var model = new EnrollViewModel()
				{
					CourseId = course.CourseId,
					User_Id = Guid.Parse(User.Identity.GetUserId())
				};
				ViewBag.IsEnrolled = _context.UserCourses.Any(uc => uc.CourseId == model.CourseId && uc.UserId == model.User_Id);
				if (ViewBag.IsEnrolled)
				{
					var testIds = _context.Tests.Where(t => t.CourseId == course.CourseId).Select(t => t.TestId).ToList();
					var testResults = _context.TestResults.Where(tr => tr.UserCourseId == _context.UserCourses.FirstOrDefault(uc => uc.CourseId == model.CourseId && uc.UserId == model.User_Id).UserCourseId && testIds.Contains(tr.TestId.Value)).ToList();
					ViewBag.TestResults = testResults;
					ViewBag.CurrentPoints = testResults.Sum(tr => tr.PointCount);
					ViewBag.MaxPoints = testResults.Count() * TestController.QUESTION_COUNT_PER_TEST;
					ViewBag.HaveTestSkipped = testResults.Any(tr => tr.CurrentTryCount == 0);
					var userCourseId = testResults.FirstOrDefault().UserCourseId;
					ViewBag.UserCourseId = userCourseId;
					//ViewBag.FinalScore = _context.UserCourses.Find(userCourseId).
					ViewBag.CompletelyFinished = _context.UserCourses.Find(userCourseId).DateCompleted != null;
					ViewBag.IsCourseFinished = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == model.CourseId && uc.UserId == model.User_Id)?.IsFinished;
					ViewBag.FinalTestResultId = _context.TestResults.FirstOrDefault(tr => tr.UserCourseId == userCourseId && tr.TestId == null)?.TestResultId;
				}
				
				return View(model);
			}
			
			return View();
		}

		[HttpPost]
		public ActionResult Enroll(EnrollViewModel model)
		{
			var enrollment = new UserCourse()
			{
				UserId = model.User_Id,
				CourseId = model.CourseId,
				IsFinished = false,
				CurrentPoints = 0
			};
			var newUserCourseId = _context.UserCourses.Add(enrollment).UserCourseId;
			var tests = _context.Tests.Where(t => t.CourseId == model.CourseId);

			tests.ToList().ForEach((test) => _context.TestResults.Add(new TestResult
			{
				CurrentTryCount = 0,
				MaxTryCount = 2,
				TestId = test.TestId,
				UserCourseId = newUserCourseId
			}));

			_context.SaveChanges();

			return RedirectToAction("Course", new { id = model.CourseId });
		}

		[Authorize]
		[HttpGet]
		public ActionResult Chapter(int id)
		{
			var chapter = _context.Tests.Find(id);
			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourse = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == chapter.CourseId && uc.UserId == userId);
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == id && tr.UserCourseId == userCourse.UserCourseId);

			ViewBag.IsCourseFinished = userCourse.IsFinished;
			ViewBag.LatestTestResult = testResult;
			ViewBag.Chapter = chapter;
			ViewBag.IsLastChapter = chapter.CourseId != _context.Tests.Find(id + 1)?.CourseId;
			ViewBag.IsFirstChapter = chapter.CourseId != _context.Tests.Find(id - 1)?.CourseId;

			return View();
		}

		[Authorize]
		[HttpGet]
		public ActionResult FinishCourse(int id)
		{
			var userCourse = _context.UserCourses.Find(id);
			ViewBag.CourseName = _context.Courses.Find(userCourse.CourseId).Title;
			ViewBag.CurrentPoints = userCourse.CurrentPoints;
			ViewBag.MaxPoints = TestController.QUESTION_COUNT_PER_TEST * _context.Tests.Where(t => t.CourseId == userCourse.CourseId).Count();

			var model = new FinishCourseViewModel()
			{
				UserCourseId = userCourse.UserCourseId
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult FinishCourse(FinishCourseViewModel model)
		{
			// add default test result with null testId
			var defaultFinalTestResult = new TestResult()
			{
				CurrentTryCount = 0,
				MaxTryCount = 1,
				UserCourseId = model.UserCourseId
			};
			_context.TestResults.Add(defaultFinalTestResult);

			// mark user course as finished
			var userCourse = _context.UserCourses.Find(model.UserCourseId);
			userCourse.IsFinished = true;
			_context.Entry(userCourse).State = System.Data.Entity.EntityState.Modified;

			_context.SaveChanges();

			return RedirectToAction("Course", new { id = userCourse.CourseId });
		}
    }
}