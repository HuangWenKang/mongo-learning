namespace MongoLearning.BsonDoc
{
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class SettingsCascades : AssertionHelper
	{
		[Test]
		public void WriteConcernAcknowledgedWithMongoClient()
		{
			var client = new MongoClient("mongodb://localhost");

			var collection = client.GetServer().GetDatabase("database").GetCollection("collection");

			Expect(collection.Settings.WriteConcern.ToString(), Is.EqualTo(WriteConcern.Acknowledged.ToString()));
		}

		[Test]
		public void WriteConcernCascadesToCollectionFromClient()
		{
			var settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost"));
			settings.WriteConcern = WriteConcern.W4;
			var client = new MongoClient(settings);

			var collection = client.GetServer().GetDatabase("database").GetCollection("collection");

			Expect(collection.Settings.WriteConcern.ToString(), Is.EqualTo(WriteConcern.W4.ToString()));
		}

		[Test]
		public void WriteConcernUnacknowledgedWithMongoServer()
		{
			var collection = MongoServer.Create("mongodb://localhost").GetDatabase("database").GetCollection("collection");

			Expect(collection.Settings.WriteConcern.ToString(), Is.EqualTo(WriteConcern.Unacknowledged.ToString()));
		}
	}
}