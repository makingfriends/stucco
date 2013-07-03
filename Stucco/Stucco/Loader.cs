using System;
using SimpleJson;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Stucco
{
	// forward declaration
	public partial interface INode
	{
	}

	public class Loader
	{
		string filename;
		IDictionary<string, object> fileData;

		public Loader(string jsonFilename)
		{
			filename = jsonFilename;
		}

		public bool ReadFile()
		{
			if (fileData == null) {
				var txt = File.ReadAllText(filename);
				object obj = SimpleJson.SimpleJson.DeserializeObject(txt);
				if (!(obj is IDictionary<string, object>)) {
					throw new SerializationException("Loader expects the top-level json primative to be an object");
				}
				fileData = obj as IDictionary<string, object>;
				if (fileData == null) {
					throw new Exception("Failed to load json file");
				}
			}
			return true;
		}

		public T Parse<T>() where T : INode
		{
			ReadFile();
			return Node.Construct<T>(fileData);
		}
	}
	public delegate void NodeChildVisitor<T>(T node);
	public partial interface INode
	{
		//		void Configure(string key, string value);
		//
		//		void Configure(string key, int value);
		//
		//		void Configure(string key, float value);
		//
		void AddChild<T>(T child) where T : INode;

		void VisitChildren<T>(NodeChildVisitor<T> visitor);
	}

	public class Node : INode
	{
		IDictionary<Type, List<object>> _children;

		public IDictionary<Type, List<object>> Children {
			get {
				if (_children == null) {
					_children = new DefaultDictionary<Type, List<object>>(new Func<List<object>>(delegate () {
						return new List<object>();
					}));
				}
				return _children;
			}
		}

		public static T Construct<T>(IDictionary<string, object> content) where T : INode
		{
			object tname;
			if (!content.TryGetValue("type", out tname)) {
				throw new Exception("All AST nodes must have a type in " + content);
			}
			if (!(tname is string)) {
				throw new Exception("Type name must be a string");
			}
			string name = tname as string;
			Type iface = TypeRegistry.GetInterface(name);
			T val = TypeRegistry.GetInstance<T>(iface);
			return val;
		}

		public void AddChild<T>(T child) where T : INode
		{
			Type kind = typeof(T);
			Children[kind].Add(child);
			Type[] subkinds = child.GetType().GetInterfaces();
			foreach (Type subkind in subkinds) {
				if (subkind != kind && subkind != typeof(INode)) {
					Children[subkind].Add(child);
				}
			}
		}

		public void VisitChildren<T>(NodeChildVisitor<T> visitor)
		{
			foreach (T item in _children[typeof(T)]) {
				Console.WriteLine("visiting: " + item);
				if (item != null) {
					visitor(item);
				}
			}
		}
	}
}
