using GeoCourse.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoCourse.Client.Controllers
{
	public class HomeController : Controller
	{
		private ApplicationDbContext _context;

		public HomeController()
		{
			_context = new ApplicationDbContext();
		}

		public ActionResult Index()
		{
			var popularCourses = _context.Courses.Take(3).AsQueryable();
			ViewBag.PopularCourses = popularCourses;
			return View();
		}

		public ActionResult All()
		{
			ViewBag.All = _context.Courses.AsQueryable();
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		
	}
}