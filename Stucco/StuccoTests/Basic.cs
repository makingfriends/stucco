using System;
using NUnit.Framework;

namespace StuccoTests
{
	[TestFixture]
	public class Basic
	{
		[Test]
		public void Pass()
		{
			Stucco.Loader loader = new Stucco.Loader("fart");
			Assert.True(true);
		}

		[Test]
		public void Fail()
		{
			Assert.False(true);
		}

		[Test]
		[Ignore ("another time")]
		public void Ignore()
		{
			Assert.True(false);
		}
	}
}
