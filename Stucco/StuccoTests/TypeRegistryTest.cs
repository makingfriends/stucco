using System;
using NUnit.Framework;
using StuccoTests.AST;

namespace StuccoTests
{
	public interface IGameObject
	{
	}

	public interface IFart : IGameObject
	{
	}

	[TestFixture]
	public class TypeRegistryTest
	{
		[Test]
		public void NonImplementedInterfaceCanNotBeInstantiated()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.TypeRegistry.GetInstance<UnimplementedInterface>(
					Stucco.TypeRegistry.GetInterface("StuccoTests.AST.UnimplementedInterface")
				);
			});
		}

		[Test]
		public void TypeRegistryGetDefaultTypeImplementation()
		{
			Assert.DoesNotThrow(delegate {
				Stucco.TypeRegistry.GetInstance<ILevel>(); // it should return the first implementation if there is only one
			});
		}

		[Test]
		public void ProvidingTheIncorrectBaseToGetInstancePukes()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.TypeRegistry.GetInstance<SomeRandomInterface>("StuccoTests.AST.IPerson");
			});
		}

		[Test]
		public void RegistryGetInterfaceWorks()
		{
			Assert.DoesNotThrow(delegate {
				Type typ = Stucco.TypeRegistry.GetInterface("StuccoTests.AST.IPerson");
				Assert.AreEqual(typ.ToString(), typeof(IPerson).ToString());
			});
		}

		[Test]
		public void RegistryGetInterfacePukesWhenRetValIsNotAnInterface()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.TypeRegistry.GetInterface("StuccoTests.AST.TestPerson");
			});
		}

		[Test]
		public void RegistryGetImplementationPukesWhenGivenNameIsNotInstantiable()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.TypeRegistry.GetInstance<IPerson>("StuccoTests.AST.IPerson");
			});
		}

		[Test]
		public void RegistryGetInterfacePukesOnInvalidName()
		{
			Assert.Throws<Stucco.UnknownTypeException>(delegate {
				Stucco.TypeRegistry.GetInterface("StuccoTests.AST.FARTBUTT");
			});
		}

		[Test]
		public void GetInterfaceType()
		{
			Assert.DoesNotThrow(delegate {
				// get IFart
				Type t = Stucco.TypeRegistry.GetAllImplementationsOfInterface<IGameObject>()[0];
			});
		}
	}
}
