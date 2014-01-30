namespace MongoLearning.BsonDoc
{
	using System;
	using System.Text.RegularExpressions;
	using MongoDB.Bson;
	using MongoDB.Bson.IO;
	using NUnit.Framework;

	[TestFixture]
	public class BsonDocumentSerialization : BsonTests
	{
		[Test]
		public void ToString_SerializesToJson()
		{
			// this just demonstrates that ToString prints a json like format, as expected since ToString calls ToJson :)
			var document = new BsonDocument();

			Console.WriteLine(document);

			Expect(document.ToString(), Is.EqualTo(document.ToJson()));
		}

		[Test]
		public void ShowcaseDifferentWaysToAddElements()
		{
			var document = new BsonDocument
			{
				{"name", "bob"}
			};
			document.Add(new BsonElement("name2", new BsonString("bob")));
			document.Add("name3", new BsonString("bob"));
			// uses implicit cast to BsonString (see implicit cast on BsonString type)
			document.Add("name4", "bob");

			Console.WriteLine(document);
		}

		[Test]
		public void ShowcaseAddressArrayViaBsonArray()
		{
			var document = new BsonDocument
			{
				{"address", new BsonArray(new[] {"100 25th Street", "Unit 1240"})}
			};

			Console.WriteLine(document);
		}

		[Test]
		public void ShowcaseBsonTypes()
		{
			var now = DateTime.UtcNow;
			var objectId = ObjectId.GenerateNewId();
			var document = new BsonDocument
			{
				{"BsonBoolean", new BsonBoolean(true)},
				{"BsonBooleanImplicit", true},
				{"BsonDateTime", new BsonDateTime(now)},
				{"BsonDateTimeImplicit", now},
				{"BsonDouble", new BsonDouble(1.01)},
				{"BsonDoubleImplicit", 1.01},
				{"BsonInt32", new BsonInt32(1)},
				{"BsonInt32Implicit", 1},
				{"BsonInt64", new BsonInt64(2)},
				{"BsonInt64Implicit", (long) 2},
				{"BsonObjectId", new BsonObjectId(objectId)},
				{"BsonObjectIdImplicit", objectId},
				{"BsonString", new BsonString("text")},
				{"BsonStringImplicit", "text"},
				{"BsonNull", BsonNull.Value},
				{"BsonRegularExpression", new BsonRegularExpression(new Regex(@".*"))},
				{"BsonRegularExpressionImplicit", new Regex(@".*")},
				{"BsonTimestamp", new BsonTimestamp(100)},
				{"BsonBinary", new BsonBinaryData(new[] {(byte) 2})},
				{"BsonArray", new BsonArray(new[] {1, 2, 3})},
				{"BsonArrayOfDocs", new BsonArray(new[] {new BsonDocument(), new BsonDocument()})},
				{"BsonDocument", new BsonDocument()},
				{"BsonJavascript", new BsonJavaScript("var a = 1;")}
			};

			Console.WriteLine(ToPrettyJson(document));
			Console.WriteLine(ToPrettyStrictJson(document));
			Console.WriteLine(ToPrettyJavascriptJson(document));
		}

		private static string ToPrettyJavascriptJson(BsonDocument document)
		{
			// javascript means what can be used in javascript as an object
			return document.ToJson(new JsonWriterSettings {OutputMode = JsonOutputMode.JavaScript, Indent = true});
		}

		private static string ToPrettyStrictJson(BsonDocument document)
		{
			// strict mode is pure JSON only
			return document.ToJson(new JsonWriterSettings {OutputMode = JsonOutputMode.Strict, Indent = true});
		}

		private static string ToPrettyJson(BsonDocument document)
		{
			return document.ToJson(new JsonWriterSettings {Indent = true});
		}

		[Test]
		public void ShowcaseToBsonInHexForamt()
		{
			var document = new BsonDocument();
			PrintBson(document);

			document.Add("name", "bob");
			PrintBson(document);
		}
	}
}