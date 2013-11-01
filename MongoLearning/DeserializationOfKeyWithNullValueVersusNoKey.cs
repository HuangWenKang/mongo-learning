namespace MongoLearning
{
	using System.Collections.Generic;
	using System.Linq;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	/// <summary>
	///     - These test cases help show that NO key doesn't impact deserialization, but NULL does,
	///     - NO key is not deserialized as NULL
	///     - If you want to ensure a collection isn't null, always make sure it has a valid value in code (set in constructor and
	///     encapsulated to block consumers from setting null) and then if you have documents with null values, just drop the
	///     key to migrate them. See the last test case for how to migrate data to drop keys with null values
	///     - Dealing with nulls is frustrating with enumerables where we want to use LINQ operators that blow up on null
	///     Enumerables.
	///		- In general null checking can be a PITA, best to understand how it affects serialization to avoid null check insanity in code
	///     - Samples with BsonIgnoreIfNull demonstrate this will block serializing null, but it doesn't block
	///     deserializing null
	///     - Resource on serialization
	///     http://docs.mongodb.org/ecosystem/tutorial/serialize-documents-with-the-csharp-driver/#serialize-documents-with-the-csharp-driver
	/// </summary>
	[TestFixture]
	public class DeserializationOfKeyWithNullValueVersusNoKey : TestsBase
	{
		private const string FamilyCollection = "Family";

		public class Family : MongoAggregate
		{
			public IEnumerable<string> Members { get; set; }
		}

		public class FamilyDefaultMembersToEmpty : MongoAggregate
		{
			public IEnumerable<string> Members { get; set; }

			public FamilyDefaultMembersToEmpty()
			{
				Members = Enumerable.Empty<string>();
			}
		}

		public class FamilyIgnoreNullsAndDefaultMembersToEmpty : MongoAggregate
		{
			[BsonIgnoreIfNull]
			public IEnumerable<string> Members { get; set; }

			public FamilyIgnoreNullsAndDefaultMembersToEmpty()
			{
				Members = Enumerable.Empty<string>();
			}
		}

		#region BsonDocument has Members Key with Null Value

		[Test]
		public void SerializeFamily_WithNullMembers_DocumentHasMembersKeyWithNullValue()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<Family>(FamilyCollection);
			var family = new Family();
			families.Save(family);

			var familyDocument = database.GetCollection(FamilyCollection).FindOneById(family.Id);

			Expect(familyDocument["Members"], Is.EqualTo(BsonNull.Value));
		}

		[Test]
		public void SerializeFamilyIgnoreNullsAndDefaultMembersToEmpty_WithNullMembers_DocumentDoesNotHaveMembersKey()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<FamilyIgnoreNullsAndDefaultMembersToEmpty>(FamilyCollection);
			var family = new FamilyIgnoreNullsAndDefaultMembersToEmpty {Members = null};
			families.Save(family);

			var familyDocument = database.GetCollection(FamilyCollection).FindOneById(family.Id);

			// BsonIgnoreIfNull blocks serializing null values
			Expect(familyDocument.Contains("Members"), Is.False);
		}

		[Test]
		public void DeserializeFamily_DocumentHasMembersKeyWithNullValue_MembersIsNull()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<Family>(FamilyCollection);
			var id = ObjectId.GenerateNewId();
			var familyDocument = new BsonDocument
			{
				{"_id", id},
				{"Members", BsonNull.Value}
			};
			families.Save(familyDocument);

			var familyDeserialized = families.FindOneById(id);
			// this case is just to compare with the following test that has a default value for Members
			Expect(familyDeserialized.Members, Is.Null);
		}

		[Test]
		public void DeserializeFamilyDefaultMembersToEmpty_DocumentHasMembersKeyWithNullValue_MembersIsNull()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<FamilyDefaultMembersToEmpty>(FamilyCollection);
			var id = ObjectId.GenerateNewId();
			var familyDocument = new BsonDocument
			{
				{"_id", id},
				{"Members", BsonNull.Value}
			};
			families.Save(familyDocument);

			var familyDeserialized = families.FindOneById(id);

			// a null value will be deserialized and replace the default
			Expect(familyDeserialized.Members, Is.Null);
		}

		[Test]
		public void DeserializeFamilyIgnoreNullsAndDefaultMembersToEmpty_DocumentHasMembersKeyWithNullValue_MembersIsNull()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<FamilyIgnoreNullsAndDefaultMembersToEmpty>(FamilyCollection);
			var id = ObjectId.GenerateNewId();
			var familyDocument = new BsonDocument
			{
				{"_id", id},
				{"Members", BsonNull.Value}
			};
			families.Save(familyDocument);

			var familyDeserialized = families.FindOneById(id);

			// BsonIgnoreIfNull doesn't block deserializing null
			Expect(familyDeserialized.Members, Is.Null);
		}

		#endregion

		#region BsonDocument doesn't have Members key

		// note: there are two ways you wouldn't have a key, either you added a new key or you previously were ignoring a key with BsonIgnoreAttribute

		[Test]
		public void DeserializeEnumerable_DocumentDoesNotHaveMembersKey_MembersIsNull()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<Family>(FamilyCollection);
			var id = ObjectId.GenerateNewId();
			var familyDocument = new BsonDocument {{"_id", id}};
			families.Save(familyDocument);

			var familyDeserialized = families.FindOneById(id);

			// this case is just to compare with the following test that has a default value for Members
			Expect(familyDeserialized.Members, Is.Null);
		}

		[Test]
		public void DeserializeEnumerable_DocumentDoesNotHaveMembersKeyAndFamilySetsEmptyInConstructor_MembersIsNotNull()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection<FamilyDefaultMembersToEmpty>(FamilyCollection);
			var id = ObjectId.GenerateNewId();
			var familyDocument = new BsonDocument {{"_id", id}};
			families.Save(familyDocument);

			var familyDeserialized = families.FindOneById(id);

			Expect(familyDeserialized.Members, Is.Not.Null);
		}

		#endregion

		[Test]
		public void MigrationToDropNullKeys()
		{
			var database = GetTestDatabase();
			database.DropCollection(FamilyCollection);
			var families = database.GetCollection(FamilyCollection);
			var documentNullMembers = new BsonDocument {{"_id", ObjectId.GenerateNewId()}, {"Members", BsonNull.Value}};
			families.Save(documentNullMembers);
			var documentNotNullMembers = new BsonDocument {{"_id", ObjectId.GenerateNewId()}, {"Members", "SomeNonNullValue"}};
			families.Save(documentNotNullMembers);

			Expect(families.FindOneById(documentNullMembers["_id"]).Contains("Members"));
			Expect(families.FindOneById(documentNotNullMembers["_id"]).Contains("Members"));

			var update = @"db.Family.update({'Members': null}, {$unset : {'Members': 1}}, {multi: true})";
			database.Eval(new BsonJavaScript(update));

			Expect(families.FindOneById(documentNullMembers["_id"]).Contains("Members"), Is.False);
			Expect(families.FindOneById(documentNotNullMembers["_id"]).Contains("Members"));
		}
	}
}