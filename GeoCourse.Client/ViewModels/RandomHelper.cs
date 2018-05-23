using GeoCourse.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoCourse.Client.ViewModels
{
	public static class RandomHelper
	{
		public static IList<QuestionViewModel> GetRandomItems(this IList<QuestionViewModel> collection, int count)
		{
			if (collection.Count <= count)
			{
				return collection;
			}

			var r = new Random();
			var start = collection.First().QuestionId;
			var end = collection.Last().QuestionId + 1;
			var collectionCopy = new List<QuestionViewModel>();
			collectionCopy.AddRange(collection);

			IList<QuestionViewModel> result = new List<QuestionViewModel>();
			while (result.Count != count)
			{
				var index = r.Next(start, end);
				if (!result.Select(res => res.QuestionId).Contains(index))
				{
					result.Add(collection.FirstOrDefault(item => item.QuestionId == index));
				}
			}

			return result;
		}

		public static IList<Question> GetRandomItems(this IList<Question> collection, int count)
		{
			var r = new Random();
			var start = collection.First().QuestionId;
			var end = collection.Last().QuestionId;

			IList<Question> result = new List<Question>();
			while (result.Count != count)
			{
				var index = r.Next(start, end);
				if (!result.Select(res => res.QuestionId).Contains(index))
				{
					result.Add(collection.FirstOrDefault(item => item.QuestionId == index));
				}
			}

			return result;
		}
	}
}