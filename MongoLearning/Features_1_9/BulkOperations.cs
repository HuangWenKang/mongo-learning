namespace MongoLearning.Features_1_9
{
    using System;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using NUnit.Framework;

    [TestFixture]
    public class BulkOperations : TestsBase
    {
        private MongoCollection<BsonDocument> _Collection;

        [SetUp]
        public void BeforeEachTest()
        {
            _Collection = GetTestDatabase().GetCollection("bulkops");
            _Collection.Drop();
        }

        [Test]
        public void Insert()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", "1"}});
            bulk.Insert(new BsonDocument {{"insert", "2"}});
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void RemoveAll()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", "1"}});
            bulk.Insert(new BsonDocument {{"insert", "2"}});
            bulk.Find(Query.Null).Remove();
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void RemoveOne()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", "1"}});
            bulk.Insert(new BsonDocument {{"insert", "2"}});
            bulk.Find(Query.Null).RemoveOne();
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void ReplaceOne()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", "1"}});
            bulk.Insert(new BsonDocument {{"insert", "2"}});
            bulk.Find(new QueryDocument {{"insert", "2"}})
                .ReplaceOne(new BsonDocument {{"insert", "2.1"}});
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void Upsert()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Find(new QueryDocument {{"_id", "money"}})
                .Upsert()
                .UpdateOne(Update.Inc("counter", 1));
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void InterestingUpsert()
        {
            // doesn't use query document if update document is empty
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Find(new QueryDocument {{"_id", "money"}})
                .Upsert()
                .UpdateOne(new UpdateDocument());
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void Update_Multiple()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", 1}});
            bulk.Insert(new BsonDocument {{"insert", 2}});
            bulk.Find(Query.Null)
                .Update(Update.Inc("insert", 10));
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        public void UpdateOne()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"insert", 1}});
            bulk.Insert(new BsonDocument {{"insert", 2}});
            bulk.Find(Query.Null)
                .UpdateOne(Update.Inc("insert", 10));
            bulk.Execute(_Collection.Settings.WriteConcern);
        }

        [Test]
        [ExpectedException(typeof (BulkWriteException))]
        public void OrderedInsertWithErrors_StopsProcessing()
        {
            var bulk = _Collection.InitializeOrderedBulkOperation();
            bulk.Insert(new BsonDocument {{"_id", 1}});
            bulk.Insert(new BsonDocument {{"_id", 2}});
            bulk.Insert(new BsonDocument {{"_id", 2}, {"document", "withAlreadyUsedId"}});
            bulk.Insert(new BsonDocument {{"_id", 3}});
            try
            {
                bulk.Execute(_Collection.Settings.WriteConcern);
            }
            catch (BulkWriteException exception)
            {
                WriteErrors(exception);
                throw;
            }
        }

        private void WriteErrors(BulkWriteException exception)
        {
            exception.WriteErrors
                .ToList()
                .ForEach(e => Console.WriteLine(e.Message));
        }

        [Test]
        [ExpectedException(typeof (BulkWriteException))]
        public void UnorderedInsertWithErrors_ProcessesEverything()
        {
            var bulk = _Collection.InitializeUnorderedBulkOperation();
            bulk.Insert(new BsonDocument {{"_id", 1}});
            bulk.Insert(new BsonDocument {{"_id", 2}});
            bulk.Insert(new BsonDocument {{"_id", 2}, {"document", "withAlreadyUsedId"}});
            bulk.Insert(new BsonDocument {{"_id", 3}});
            try
            {
                bulk.Execute(_Collection.Settings.WriteConcern);
            }
            catch (BulkWriteException exception)
            {
                WriteErrors(exception);
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof (BulkWriteException))]
        public void ShowOffDirectBulkWriteToSpecifyOtherArguments()
        {
            var args = new BulkWriteArgs
            {
                CheckElementNames = false
            };
            var insert = new InsertRequest(typeof (BsonDocument), new BsonDocument {{"$invalid", 1}});
            try
            {
                _Collection.BulkWrite(args, insert);
            }
            catch (BulkWriteException exception)
            {
                WriteErrors(exception);
                throw;
            }
        }

        [TearDown]
        public void AfterEachTest()
        {
            Console.WriteLine("Documents:");
            _Collection.FindAll()
                .ToList()
                .ForEach(Console.WriteLine);
        }
    }
}