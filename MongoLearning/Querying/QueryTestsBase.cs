namespace MongoLearning.Querying
{
	using MongoDB.Bson;
	using MongoDB.Driver;
	using NUnit.Framework;

	public class QueryTestsBase : TestsBase
	{
		protected MongoCollection<BsonDocument> QueryTests;

		[SetUp]
		public void BeforeEachTest()
		{
			QueryTests = GetTestDatabase().GetCollection("querytests");
			QueryTests.Drop();
		}
	}
}