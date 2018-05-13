namespace GeoCourse.Client.Models
{
	public class TestResult
	{
		public int TestResultId { get; set; }

		public int? TestId { get; set; }

		public Test Test { get; set; }

		public int? UserCourseId { get; set; }

		public virtual UserCourse UserCourse { get; set; }

		public int PointCount { get; set; }

		public int CurrentTryCount { get; set; }

		public int MaxTryCount { get; set; }
	}
}