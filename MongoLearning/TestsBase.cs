namespace MongoLearning
{
	using MongoDB.Driver;
	using NUnit.Framework;

	public class TestsBase : AssertionHelper
	{
		protected static MongoDatabase GetTestDatabase()
		{
			var settings = new MongoClientSettings
			{
				Server = new MongoServerAddress("localhost")
			};
			return new MongoClient(settings).GetServer().GetDatabase("mongo-learning");
		}
	}
}