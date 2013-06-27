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
	public delegate void NodeChildVisitor(INode node);
	public partial interface INode
	{
		//		void Configure(string key, string value);
		//
		//		void Configure(string key, int value);
		//
		//		void Configure(string key, float value);
		//
		//		void AddChild(INode child);
		//
		//		void VisitChildren(NodeChildVisitor visitor);
	}

	public class Node : INode
	{
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
			INode val = TypeRegistry.GetInstance<T>(iface);
			return (T)val;
		}
	}
}
