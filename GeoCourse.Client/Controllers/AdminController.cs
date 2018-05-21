using GeoCourse.Client.Models;
using GeoCourse.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoCourse.Client.Controllers
{
	[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
		private ApplicationDbContext _context;

		public AdminController()
		{
			_context = new ApplicationDbContext();
		}

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public ActionResult Users()
		{
			ViewBag.Users = _context.Users.ToList();

			return View();
		}

		[HttpGet]
		public ActionResult UserInfo(string id)
		{
			var userName = _context.Users.Find(id).UserName;
			ViewBag.UserName = userName;
			var userCourses = _context.UserCourses.Where(uc => uc.UserId.ToString() == id).ToList();

			if (userCourses.Any())
			{
				var userStatsListViewModel = new UserStatsListViewModel()
				{
					//UserStats = new List<UserStatsViewModel>()
					UserStats = userCourses.Select(uc => {

						var course = _context.Courses.Find(uc.CourseId);
						var testResults = _context.TestResults.Where(tr => tr.TestId != null && tr.UserCourseId == uc.UserCourseId).ToList();
						var maxAttemptes = testResults.Count * 2;
						var currentAttempts = testResults.Sum(tr => tr.CurrentTryCount);
						var testTried = testResults.Where(tr => tr.CurrentTryCount != 0).Count();

						return new UserStatsViewModel()
						{
							CourseTitle = course.Title,
							State = uc.IsCompleted.HasValue ? uc.IsCompleted.Value ? "Успешно пройден" : "Провален" : "В процессе изучения",
							PointProgress = $"{uc.CurrentPoints}/{course.MaxPoints}",
							AttemptProgress = $"{currentAttempts}/{maxAttemptes}",
							TestProgress = $"{testTried}/{testResults.Count}",
							RequiredPercent = $"{course.RequiredPoints} %",
							FinalTestResult = uc.DateCompleted == null ? "Ещё не пройден" : $"{_context.TestResults.FirstOrDefault(tr => tr.TestId == null && tr.UserCourseId == uc.UserCourseId).PointCount}/{testResults.Count * 3}",
							FinalCourseScore = uc.IsCompleted.HasValue ? $"{uc.FinalCourseScore}" : "Курс еще не завершен"
						};
					}).ToList()
				};

				//foreach (var userCourse in userCourses)
				//{
				//	var course = _context.Courses.Find(userCourse.CourseId);
				//	var testResults = _context.TestResults.Where(tr => tr.TestId != null && tr.UserCourseId == userCourse.UserCourseId).ToList();
				//	var maxAttemptes = testResults.Count * 2;
				//	var currentAttempts = testResults.Sum(tr => tr.CurrentTryCount);
				//	var testTried = testResults.Where(tr => tr.CurrentTryCount != 0);

				//	var userStats = new UserStatsViewModel()
				//	{
				//		CourseTitle = course.Title,
				//		State = userCourse.IsCompleted.HasValue ? userCourse.IsCompleted.Value ? "Успешно пройден" : "Провален" : "В процессе изучения",
				//		PointProgress = $"{userCourse.CurrentPoints}/{course.MaxPoints}",
				//		AttemptProgress = $"{currentAttempts}/{maxAttemptes}",
				//		TestProgress = $"{testTried}/{testResults.Count}",
				//		RequiredPercent = $"{course.RequiredPoints} %",
				//		FinalTestResult = userCourse.DateCompleted == null ? "Ещё не пройден" : $"{_context.TestResults.FirstOrDefault(tr => tr.TestId == null && tr.UserCourseId == userCourse.UserCourseId).PointCount}/{testResults.Count * 3}",
				//		FinalCourseScore = userCourse.IsCompleted.HasValue ? $"{userCourse.FinalCourseScore}" : "Курс еще не завершен"
				//	};
				//	userStatsListViewModel.UserStats.Add(userStats);
				//}

				return View(userStatsListViewModel);
			}
			else
			{
				return View(new UserStatsListViewModel() { UserStats = new List<UserStatsViewModel>() });
			}
		}
    }
}