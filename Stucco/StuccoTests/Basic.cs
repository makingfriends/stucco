using System;
using NUnit.Framework;
using System.IO;

namespace StuccoTests
{
	[TestFixture]
	public class Basic
	{
		[Test]
		public void LoadSomeJSON()
		{
			Assert.DoesNotThrow(delegate {
				(new Stucco.Loader("./TestData/valid_json.json")).Begin();
			});
		}

		[Test]
		public void PukeOnInvalidJSON()
		{
			// bad syntax
			Assert.Throws<System.Runtime.Serialization.SerializationException>(delegate {
				(new Stucco.Loader("./TestData/invalid_json.json")).Begin();
			});

			// root object must be an object
			Assert.Throws<System.Runtime.Serialization.SerializationException>(delegate {
				(new Stucco.Loader("./TestData/invalid_json2.json")).Begin();
			});
		}
	}
}
