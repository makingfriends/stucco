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

	public class TestLevel : Stucco.Node, ILevel
	{
	}

	public class TestPerson : Stucco.Node, IPerson
	{
	}

	public class TestTree : Stucco.Node, ITree
	{
	}

	public interface SomeRandomInterface : Stucco.INode
	{
	}

	public class SomeRandomImplementation : Stucco.Node, SomeRandomInterface
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
				IGameObject go = Stucco.Node.Construct<IGameObject>(d);
				Assert.True(go != null);
				Assert.True(go is ITree);
				Assert.True(go is TestTree);
			});
		}

		[Test]
		public void CanAddChild()
		{
			var d = new Dictionary<string, object> { { "type", "StuccoTests.AST.ITree" } };
			IGameObject go = Stucco.Node.Construct<IGameObject>(d);
			var p = new Dictionary<string, object> { { "type", "StuccoTests.AST.IPerson" } };
			IGameObject po = Stucco.Node.Construct<IGameObject>(p);

			// first, ensure that we don't puke while adding the child
			Assert.DoesNotThrow(delegate {
				go.AddChild<IGameObject>(po);
			});

			// then, ensure that we can find the child that was just added.
			bool found = false;
			go.VisitChildren<IGameObject>(delegate(IGameObject person) {
				found = true;
				Assert.True(person == po);
			});
			Assert.True(found, "couldn't find the child that was added");
		}

		[Test]
		public void ChildTypesCascade()
		{
			var d = new Dictionary<string, object> { { "type", "StuccoTests.AST.ITree" } };
			IGameObject go = Stucco.Node.Construct<IGameObject>(d);
			var p = new Dictionary<string, object> { { "type", "StuccoTests.AST.IPerson" } };
			IGameObject po = Stucco.Node.Construct<IGameObject>(p);

			go.AddChild<IGameObject>(po); // this should add a child entry for IGameObject and IPerson

			// child types must be accessible by both 
			bool found = false;
			go.VisitChildren<IPerson>(delegate(IPerson person) { // essentially meaning, get all people
				found = true;
				Assert.True(person == po, "did not refer to the same value");
			});
			Assert.True(found, "couldn't find the child node that was added");
		}

		[Test]
		public void ConstructAddsChildren()
		{
			var d = new Dictionary<string, object> {
				{ "type", "StuccoTests.AST.ILevel" },
				{ 
					"children", new List<object> { 
						new Dictionary<string, object> { 
							{ "type", "StuccoTests.AST.ITree" } 
						} 
					}
				}
			};

			IGameObject go = Stucco.Node.Construct<IGameObject>(d);

			bool found = false;
			go.VisitChildren<ITree>(delegate(ITree tree) {
				found = true;
			});

			Assert.True(found, "could not find the child that should've been added");
		}
	}
}
