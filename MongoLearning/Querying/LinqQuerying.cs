namespace MongoLearning.Querying
{
	using System;
	using System.Linq;
	using MongoDB.Bson;
	using MongoDB.Driver.Linq;
	using NUnit.Framework;

	public class LinqQuerying : QueryTestsBase
	{
		private class House
		{
			public ObjectId Id { get; set; }
			public string Description { get; set; }
		}

		[Test]
		public void LinqContains()
		{
			QueryTests.Insert(new House {Description = "this is a nice house"});
			QueryTests.Insert(new House {Description = "not it is not"});

			Console.WriteLine("houses with nice in description:");
			QueryTests.AsQueryable<House>()
				.Where(h => h.Description.Contains("nice"))
				.ToList()
				.ForEach(h => Console.WriteLine(h.Description));
		}
	}
}