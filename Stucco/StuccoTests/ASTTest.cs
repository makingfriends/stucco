using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace StuccoTests.AST
{
	/**
	 * Some test interfaces
	 */
	public interface IGameObject : Stucco.INode
	{
	}

	public interface ILevel : IGameObject
	{
	}

	public interface IPerson : IGameObject
	{
	}

	public interface ITree : IGameObject
	{
	}

	public class TestLevel : ILevel
	{
	}

	public class TestPerson : IPerson
	{
	}

	public class TestTree : ITree
	{
	}

	public interface SomeRandomInterface : Stucco.INode
	{
	}

	public class SomeRandomImplementation : SomeRandomInterface
	{
	}

	public interface UnimplementedInterface : Stucco.INode
	{
	}

	[TestFixture]
	public class ASTTest
	{
		[Test]
		public void NonImplementedInterfaceCanNotBeInstantiated()
		{
			var d = new Dictionary<string, object> {
				{ "type", "StuccoTests.AST.UnimplementedInterface" }
			};
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.Node.Construct<Stucco.INode>(d);
			});
		}

		[Test]
		public void CanInstantiateRootNode()
		{
			var d = new Dictionary<string, object> {
				{ "type", "StuccoTests.AST.ITree" }
			};
			Assert.DoesNotThrow(delegate {
				Console.WriteLine("faak");
				IGameObject go = Stucco.Node.Construct<IGameObject>(d);
				Console.WriteLine("shit");
				Assert.True(go is IGameObject);
				Assert.True(go is ITree);
				Assert.True(go is TestTree);
			});
		}
	}
}
