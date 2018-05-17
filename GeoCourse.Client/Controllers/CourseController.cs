﻿using GeoCourse.Client.Models;
using GeoCourse.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace GeoCourse.Client.Controllers
{
    public class CourseController : Controller
    {
		public ApplicationDbContext _context;

		public CourseController()
		{
			_context = new ApplicationDbContext();
		}

        // GET: Course
        public ActionResult Index()
        {
            return View();
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
				var testIds = _context.Tests.Where(t => t.CourseId == course.CourseId).Select(t => t.TestId).ToList();
				ViewBag.TestResults = _context.TestResults.Where(tr => testIds.Contains(tr.TestId.Value)).ToList();

				//ViewBag.
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

			return View("Course");
		}

		[HttpGet]
		public ActionResult Chapter(int id)
		{
			var chapter = _context.Tests.Find(id);
			var userId = Guid.Parse(User.Identity.GetUserId());
			var userCourseId = _context.UserCourses.FirstOrDefault(uc => uc.CourseId == chapter.CourseId && uc.UserId == userId).UserCourseId;
			var testResult = _context.TestResults.FirstOrDefault(tr => tr.TestId == id && tr.UserCourseId == userCourseId);

			ViewBag.LatestTestResult = testResult;
			ViewBag.Chapter = chapter;

			return View();
		}
    }
}