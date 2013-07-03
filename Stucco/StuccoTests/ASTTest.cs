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

		[Test]
		public void PukeWhenChildrenNotAnArray()
		{

		}

		[Test]
		public void PukeWhenChildrenContentsNotANode()
		{

		}

		[Test]
		public void LongProperty()
		{
			var l = new Stucco.Loader("./TestData/ast/ast_props.json");
			var go = l.Parse<IGameObject>();
			Assert.AreEqual(42, go.GetLong("i_am_an_int"));
		}

		[Test]
		public void FloatProperty()
		{
			var l = new Stucco.Loader("./TestData/ast/ast_props.json");
			var go = l.Parse<IGameObject>();
			Assert.AreEqual(4.20, go.GetDouble("i_am_a_float"));
		}

		[Test]
		public void BoolProperty()
		{
			var l = new Stucco.Loader("./TestData/ast/ast_props.json");
			var go = l.Parse<IGameObject>();
			Assert.AreEqual(false, go.GetBool("i_am_a_bool"));
		}

		[Test]
		public void StrProperty()
		{
			var l = new Stucco.Loader("./TestData/ast/ast_props.json");
			var go = l.Parse<IGameObject>();
			Assert.AreEqual("faz bat", go.GetString("i_am_a_string"));
		}

		[Test]
		public void PukeWhenPropertiesNotADictionary()
		{
			Assert.Throws<NotSupportedException>(delegate() {
				var l = new Stucco.Loader("./TestData/ast/ast_props_invalid_props.json");
				l.Parse<IGameObject>();
			});
		}

		[Test]
		public void PukeWhenPropertiesHaveInvalidTypes()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate() {
				var l = new Stucco.Loader("./TestData/ast/ast_props_invalid_typ.json");
				l.Parse<IGameObject>();
			});
		}

		[Test]
		public void PukeWhenNodeTypeIsntAString()
		{
			Assert.Throws<NotSupportedException>(delegate () {
				var l = new Stucco.Loader("./TestData/ast/ast_invalid_type_typ.json");
				l.Parse<IGameObject>();
			});
		}

		[Test]
		public void PukeWhenNodeDoesntHaveAType()
		{
			Assert.Throws<NotSupportedException>(delegate () {
				var l = new Stucco.Loader("./TestData/ast/ast_missing_type.json");
				l.Parse<IGameObject>();
			});
		}
	}
}
