namespace MongoLearning
{
	using MongoDB.Bson;

	public abstract class MongoAggregate
	{
		public ObjectId Id { get; set; }

		protected MongoAggregate()
		{
			Id = ObjectId.GenerateNewId();
		}
	}
}