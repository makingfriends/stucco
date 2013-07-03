using System;
using NUnit.Framework;
using System.IO;

namespace StuccoTests
{
	[TestFixture]
	public class LoaderTests
	{
		[Test]
		public void LoadSomeJSON()
		{
			Assert.DoesNotThrow(delegate {
				(new Stucco.Loader("./TestData/valid_json.json")).ReadFile();
			});
		}

		[Test]
		public void PukeOnInvalidJSON()
		{
			// bad syntax
			Assert.Throws<System.Runtime.Serialization.SerializationException>(delegate {
				(new Stucco.Loader("./TestData/invalid_json.json")).ReadFile();
			});

			// root object must be an object
			Assert.Throws<System.Runtime.Serialization.SerializationException>(delegate {
				(new Stucco.Loader("./TestData/invalid_json2.json")).ReadFile();
			});
		}
	}
}
