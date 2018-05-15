using GeoCourse.Client.Models;
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
				ViewBag.CourseMap = _context.Tests.Where(t => t.CourseId == course.CourseId).AsQueryable();
				var model = new EnrollViewModel()
				{
					CourseId = course.CourseId,
					User_Id = Guid.Parse(User.Identity.GetUserId())
				};
				ViewBag.IsEnrolled = _context.UserCourses.Any(uc => uc.CourseId == model.CourseId && uc.UserId == model.User_Id);
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
			_context.UserCourses.Add(enrollment);
			_context.SaveChanges();

			return View("Course");
		}

		[HttpGet]
		public ActionResult Chapter(int id)
		{
			var chapter = _context.Tests.Find(id);
			ViewBag.Chapter = chapter;

			return View();
		}
    }
}