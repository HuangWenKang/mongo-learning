namespace MongoLearning.Querying
{
	using System;
	using System.Linq;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using NUnit.Framework;

	public class Aggregation : QueryTestsBase
	{
		private class Person
		{
			public ObjectId Id { get; set; }
			public string Name { get; set; }
			public int Age { get; set; }
		}

		[Test]
		public void MatchMatchSort()
		{
			InsertPeople();

			var matchAndSort = new AggregateArgs
			{
				Pipeline = new[]
				{
					new BsonDocument("$match", Query.LT("Age", 70).ToBsonDocument()),
					new BsonDocument("$match", Query.GT("Age", 30).ToBsonDocument()),
					new BsonDocument("$sort", new BsonDocument("Age", 1))
				}
			};

			PrintResults(matchAndSort);
		}

		[Test]
		public void MatchGroupSort()
		{
			InsertPeople();

			var aggregation = new AggregateArgs
			{
				Pipeline = new[]
				{
					new BsonDocument("$match", Query.LT("Age", 70).ToBsonDocument()),
					new BsonDocument("$group", new BsonDocument
					{
						{"_id", new BsonDocument("$subtract", new BsonArray {"$Age", new BsonDocument("$mod", new BsonArray {"$Age", 10})})},
						{"averageAge", new BsonDocument("$avg", "$Age")}
					}),
					new BsonDocument("$sort", new BsonDocument("_id", 1))
				}
			};

			PrintResults(aggregation);
		}

		private void PrintResults(AggregateArgs matchAndSort)
		{
			QueryTests.Aggregate(matchAndSort)
				.ToList()
				.ForEach(Console.WriteLine);
		}

		private void InsertPeople()
		{
			QueryTests.Insert(new Person {Age = 25, Name = "jane"});
			QueryTests.Insert(new Person {Age = 60, Name = "deb"});
			QueryTests.Insert(new Person {Age = 63, Name = "bob"});
			QueryTests.Insert(new Person {Age = 74, Name = "alicia"});
			QueryTests.Insert(new Person {Age = 34, Name = "joe"});
			QueryTests.Insert(new Person {Age = 29, Name = "deb"});
			QueryTests.Insert(new Person {Age = 21, Name = "carl"});
			QueryTests.Insert(new Person {Age = 45, Name = "betty"});
		}
	}
}