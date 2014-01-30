namespace MongoLearning.BsonDoc
{
	using System;
	using System.Linq;
	using MongoDB.Bson;
	using NUnit.Framework;

	public class BsonTests : AssertionHelper
	{
		protected void PrintBson(object document)
		{
			var bytes = document.ToBson();
			// print hex encoding
			var bsonAsHex = BitConverter.ToString(bytes);
			Console.WriteLine(bsonAsHex);

			// try to print readable characters, lined up with hex encoding
			var characters = bytes
				.Select(b => (char) b)
				.Select(c => Char.IsLetterOrDigit(c) ? " " + c : "  ")
				.ToArray();
			Console.WriteLine(String.Join("-", characters));
		}
	}
}