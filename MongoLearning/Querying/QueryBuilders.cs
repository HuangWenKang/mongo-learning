namespace MongoLearning.Querying
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using MongoDB.Bson;
	using MongoDB.Driver.Builders;
	using NUnit.Framework;

	public class QueryBuilders : QueryTestsBase
	{
		[Test]
		public void Comparisons()
		{
			Console.WriteLine(Query.EQ("name", "jane"));
			Console.WriteLine(Query.In("name", new BsonValue[] {"jane", "anne"}));
			Console.WriteLine(Query.NE("name", "jane"));
			Console.WriteLine(Query.NotIn("name", new BsonValue[] {"jane", "anne"}));

			Console.WriteLine(Query.GT("age", 50));
			Console.WriteLine(Query.GTE("age", 50));
			Console.WriteLine(Query.LT("age", 50));
			Console.WriteLine(Query.LTE("age", 50));
		}

		[Test]
		public void Logical()
		{
			var and = Query.And(Query.EQ("name", "jane"), Query.EQ("age", 80));
			Console.WriteLine(and);
			var or = Query.Or(Query.EQ("name", "jane"), Query.EQ("age", 80));
			Console.WriteLine(or);
			var notAnd = Query.Not(and);
			Console.WriteLine(notAnd);
			var notOr = Query.Not(or);
			Console.WriteLine(notOr);
		}

		[Test]
		public void Element()
		{
			// helpful when adding a new few, or with an optional field
			Console.WriteLine(Query.Exists("age"));
			Console.WriteLine(Query.NotExists("age"));
			// could be used when migrating a field type
			Console.WriteLine(Query.Type("age", BsonType.String));
		}

		[Test]
		public void Evaluation()
		{
			// find all people in their 80s
			Console.WriteLine(Query.Mod("age", 10, 8));
			// find all people who have a first name starting with A
			Console.WriteLine(Query.Matches("name", new Regex("^[aA]")));

			// arbitrary javascript, slow! use in conjunction with non where queries that hit an index or really narrow down documents first
			var where = Query.Where("this.age === 1");
			Console.WriteLine(where);
			Console.WriteLine("where matches:");
			QueryTests.Insert(new BsonDocument {{"age", 2}});
			QueryTests.Insert(new BsonDocument {{"age", 1}});
			QueryTests.Find(where)
				.ToList()
				.ForEach(Console.WriteLine);
		}

		[Test]
		public void Geospatial()
		{
			// todo later
		}

		[Test]
		public void Array()
		{
			Console.WriteLine(Query.All("array", new BsonValue[] {1, 2, 3}));

			// tests at least one item in a nested array matches the nested query, like querying documents within a nested array
			var nestedQuery = Query.EQ("nestedkey", "nestedValue");
			Console.WriteLine(Query.ElemMatch("collection", nestedQuery));

			Console.WriteLine(Query.Size("array", 5));
			// hacks to check for size greather than / less than (check for numbered elements existing)
			Console.WriteLine(Query.SizeGreaterThan("array", 5));
			Console.WriteLine(Query.SizeGreaterThanOrEqual("array", 5));
			Console.WriteLine(Query.SizeLessThan("array", 5));
			Console.WriteLine(Query.SizeLessThanOrEqual("array", 5));
		}

		[Test]
		public void TextSearch()
		{
			Console.WriteLine(Query.Text("jane"));
		}

		private class Person
		{
			public string Name { get; set; }
		}

		[Test]
		public void StronglyTypedBuilder()
		{
			Console.WriteLine(Query.EQ("Name", "anne"));
			Console.WriteLine(Query<Person>.EQ(p => p.Name, "anne"));
		}

		[Test]
		public void StronglyTypedWhere_BuildsQueryFromExpression()
		{
			Console.WriteLine(Query<Person>.Where(p => p.Name == "anne"));
			Console.WriteLine(Query<Person>.Where(p => p.Name.Contains("anne")));
		}

		[Test]
		[ExpectedException(typeof (NotSupportedException))]
		public void InvalidStronglyTypedWhere()
		{
			Query<Person>.Where(p => p.Name.TrimEnd('n') == "anne");
		}
	}
}