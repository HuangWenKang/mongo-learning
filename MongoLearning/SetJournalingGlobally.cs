namespace MongoLearning
{
	using MongoDB.Driver;
	using NUnit.Framework;

	public class SetJournalingGlobally : AssertionHelper
	{
		[Test]
		public void ViaCollectionSettings()
		{
			var settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://localhost"));
			settings.WriteConcern.Journal = true;
			var collection = new MongoClient(settings)
				.GetServer().GetDatabase("mydb").GetCollection("mycollection");

			Expect(collection.Settings.WriteConcern.Journal, Is.True);
		}

		[Test]
		public void ViaConnectionString()
		{
			var collection = new MongoClient("mongodb://localhost/?journal=true")
				.GetServer().GetDatabase("mydb").GetCollection("mycollection");

			Expect(collection.Settings.WriteConcern.Journal, Is.True);
		}
	}
}