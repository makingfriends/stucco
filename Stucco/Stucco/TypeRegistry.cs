using System;
using System.Collections.Generic;

namespace Stucco
{
	public class UnknownTypeException : Exception
	{
		public UnknownTypeException(string exc) : base(exc)
		{
		}

		public UnknownTypeException() : base()
		{
		}
	}
	// look up classes. THIS IS **NOT** THREAD SAFE
	public class TypeRegistry
	{
		// first level cache (caches GetInstance<T>(string))
		private static Dictionary<string, Type> reverseLookupTable = new Dictionary<string, Type>();
		// second level cache (caches GetAllImplementationsOfInterface(Type))
		private static Dictionary<string, List<Type>> implementationCache = new Dictionary<string, List<Type>>();
		// caches interfaces by name
		private static Dictionary<string, Type> interfacesCache = new Dictionary<string, Type>();

		public static List<Type> GetAllImplementationsOfInterface<T>()
		{
			Type interfaceType = typeof(T);
			return GetAllImplementationsOfInterface(interfaceType);
		}

		public static List<Type> GetAllImplementationsOfInterface(Type interfaceType)
		{
			var typeName = interfaceType.ToString();
			if (implementationCache.ContainsKey(typeName)) {
				return implementationCache[typeName];
			}

			var result = new List<Type>();
			System.Reflection.Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in allAssemblies) {
				// this goes through thousands and thousands of types... it's probably pretty expensive on a mobile device.
				Type[] types = assembly.GetTypes();
				foreach (var inspectedType in types) {
					var interfaces = inspectedType.FindInterfaces((Type typeObj, Object criteriaObj) => {
						//						Console.WriteLine("type: " + inspectedType + " iface: " + typeObj);
						if (typeObj.ToString().Equals(typeName)) { // Type.IsEquivalentTo(Type) is unavailable in MonoTouch
							return true;
						}
						return false;
					}, null);
					if (interfaces.Length > 0) {
						result.Add(inspectedType);
					}
				}
			}
			implementationCache[typeName] = result;
			return result;
		}

		public static Type GetInterface(string iname)
		{
			if (interfacesCache.ContainsKey(iname)) {
				return interfacesCache[iname];
			}
			System.Reflection.Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in allAssemblies) {
				// this goes through thousands and thousands of types... it's probably pretty expensive on a mobile device.
				Type[] types = assembly.GetTypes();
				foreach (var inspectedType in types) {
					if (inspectedType.IsInterface && inspectedType.FullName.Equals(iname)) {
						interfacesCache[iname] = inspectedType;
						return inspectedType;
					}
				}
			}
			throw new UnknownTypeException();
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetInstance<T>()
		{
			return GetInstance<T>(typeof(T));
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="ifaceType">Iface.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetInstance<T>(Type ifaceType)
		{
			Type desiredType;
			T ret;
			if (reverseLookupTable.TryGetValue(ifaceType.ToString(), out desiredType)) {
				ret = (T)Activator.CreateInstance(desiredType);
				return ret;
			}
			List<Type> allImplementations = GetAllImplementationsOfInterface(ifaceType);
			if (allImplementations.Count < 1) {
				throw new UnknownTypeException("No implementations of: " + ifaceType);
			}
			if (allImplementations.Count > 1) {
				throw new Exception("There were more than 1 implementation of " + ifaceType + " so you must supply an implementation name");
			}
			desiredType = allImplementations[0];
			ret = (T)Activator.CreateInstance(desiredType);
			return ret;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="implementationName">Implementation name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetInstance<T>(string implementationName)
		{
			Type desiredType = null;
			if (reverseLookupTable.ContainsKey(implementationName)) {
				desiredType = reverseLookupTable[implementationName];
			} else {
				List<Type> allImplementations = GetAllImplementationsOfInterface<T>();
				Console.WriteLine("name: " + implementationName);
				foreach (Type inspectedType in allImplementations) {
					Console.WriteLine("implementation: " + inspectedType);
					if (inspectedType.IsClass && inspectedType.FullName.Equals(implementationName)) {
						desiredType = inspectedType;
						reverseLookupTable[implementationName] = desiredType;
					}
				}
			}
			if (desiredType == null) {
				throw new UnknownTypeException();
			}
			try {
				T ret = (T)Activator.CreateInstance(desiredType);
				return ret;
			} catch (MissingMethodException e) {
				// this is a weird choice of exception to throw, but it's thrown when the implementationName points 
				// to an uninstantiable type (interface, abstract class, etc)
				Console.WriteLine("missing method exception: " + e);
				throw new UnknownTypeException();
			}
		}
	}
}
