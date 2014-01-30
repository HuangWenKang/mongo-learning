namespace MongoLearning.BsonDoc
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Bson.Serialization.Serializers;
	using NUnit.Framework;

	[TestFixture]
	public class BsonSerializerApi : BsonTests
	{
		[Test]
		public void SerializingBsonDocument()
		{
			var document = new BsonDocument {{"name", "bob"}};

			// uses BsonBinaryWriter to write to a BsonBuffer
			PrintBson(document);

			// uses JsonWriter to write to a StringWriter
			Console.WriteLine(document.ToJson());
		}

		[Test]
		public void SerializingPoco()
		{
			var poco = new {name = "bob"};

			// uses BsonBinaryWriter to write to a BsonBuffer
			PrintBson(poco);

			// uses JsonWriter to write to a StringWriter
			Console.WriteLine(poco.ToJson());

			// uses BsonDocumentWriter to write to a BsonDocument
			Console.WriteLine(poco.ToBsonDocument());
		}

		[Test]
		public void DeserializeBsonDocument_FromBson()
		{
			var document = new BsonDocument
			{
				{"name", "bob"}
			};
			var bytes = document.ToBson();

			var deserialized = BsonSerializer.Deserialize<BsonDocument>(bytes);

			Expect(deserialized["name"], Is.EqualTo(new BsonString("bob")));
		}

		[Test]
		public void DeserializeBsonDocument_FromJson()
		{
			var document = new BsonDocument
			{
				{"name", "bob"}
			};
			var json = document.ToJson();

			var deserialized = BsonSerializer.Deserialize<BsonDocument>(json);

			Expect(deserialized["name"], Is.EqualTo(new BsonString("bob")));
		}

		private class Person
		{
			public string Name { get; set; }
		}

		[Test]
		public void LookupDefaultSerializers()
		{
			Expect(BsonSerializer.LookupSerializer(typeof (Person)), Is.TypeOf<BsonClassMapSerializer>());
			Expect(BsonSerializer.LookupSerializer(typeof (BsonDocument)), Is.EqualTo(BsonDocumentSerializer.Instance));
			Expect(BsonSerializer.LookupSerializer(typeof (BsonDouble)), Is.TypeOf<BsonDoubleSerializer>());
			Expect(BsonSerializer.LookupSerializer(typeof (double)), Is.TypeOf<DoubleSerializer>());

			// notes on LookupSerializer
			// if BsonDocument is type, returns BsonDocumentSerializer.Instance
			// when finding serializer, checks BsonSerializerAttribute on type first
			// checks if type is generic next
			// last, iterates through serialization providers and tries each in order to get a serializer
			// default there is BsonDefaultSerializationProvider and BsonClassMapSerializationProvider
			// BsonDefaultSerializationProvider -> handles pretty much all primitives, values (.net and Bson), also IBsonSerializable, pretty much anything not going through a class map, so anything not a class MOSTLY
			// BsonClassMapSerializationProvider -> handles pocos and generating class maps (just looks up a class map via BsonClassMap.LookupClassMap, which in turn just looks up an existing class map or creates an AutoMap on the fly)
		}

		[Test]
		public void DeserializePoco_FromBson()
		{
			var poco = new Person {Name = "bob"};
			var bytes = poco.ToBson();

			var deserialized = BsonSerializer.Deserialize<BsonDocument>(bytes);

			Expect(deserialized["Name"], Is.EqualTo(new BsonString("bob")));
		}

		[Test]
		public void DeserializePoco_FromJson()
		{
			var poco = new Person {Name = "bob"};
			var json = poco.ToJson();

			var deserialized = BsonSerializer.Deserialize<BsonDocument>(json);

			Expect(deserialized["Name"], Is.EqualTo(new BsonString("bob")));
		}

		[Test]
		public void DeserializePoco_FromBsonDocument()
		{
			var document = new BsonDocument
			{
				{"Name", "bob"}
			};

			var deserialized = BsonSerializer.Deserialize<BsonDocument>(document);

			Expect(deserialized["Name"], Is.EqualTo(new BsonString("bob")));
		}
	}
}