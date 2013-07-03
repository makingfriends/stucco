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
		void Set(string key, string value);

		void Set(string key, long value);

		void Set(string key, double value);

		void Set(string key, bool value);

		long GetLong(string key);

		double GetDouble(string key);

		bool GetBool(string key);

		string GetString(string key);

		void AddChild<T>(T child) where T : INode;

		void VisitChildren<T>(NodeChildVisitor<T> visitor) where T : INode;
	}

	public class Node : INode
	{
		IDictionary<Type, List<object>> _children;

		IDictionary<Type, List<object>> Children {
			get {
				if (_children == null) {
					_children = new DefaultDictionary<Type, List<object>>(new Func<List<object>>(delegate () {
						return new List<object>();
					}));
				}
				return _children;
			}
		}
		// todo: can make these lazy to save memory
		IDictionary<string, long> LongConfigs = new Dictionary<string, long>();
		IDictionary<string, double> DoubleConfigs = new Dictionary<string, double>();
		IDictionary<string, bool> BoolConfigs = new Dictionary<string, bool>();
		IDictionary<string, string> StrConfigs = new Dictionary<string, string>();

		public static T Construct<T>(IDictionary<string, object> content) where T : INode
		{
			// load the name
			object tname;
			if (!content.TryGetValue("type", out tname)) {
				throw new NotSupportedException("All AST nodes must have a type in " + content);
			}
			if (!(tname is string)) {
				throw new NotSupportedException("Type name must be a string");
			}
			string name = tname as string;

			// instantiate the object
			Type iface = TypeRegistry.GetInterface(name);
			T val = TypeRegistry.GetInstance<T>(iface);

			// add children
			object tchildren;
			if (content.TryGetValue("children", out tchildren)) { // "children" is optional
				if (!(tchildren is IList<object>)) {
					throw new NotSupportedException("`children` must be a list.");
				}
				List<object> children = tchildren as List<object>;
				foreach (object tchild in children) {
					if (!(tchild is IDictionary<string, object>)) {
						throw new NotSupportedException("`children` must be a list containing dictionaries.");
					}
					val.AddChild<T>(Construct<T>(tchild as IDictionary<string, object>));
				}
			}

			// set configuration values
			object tprops;
			if (content.TryGetValue("properties", out tprops)) {
				Console.WriteLine("type: " + tprops.GetType());
				if (!(tprops is IDictionary<string, object>)) {
					throw new NotSupportedException("`properties` must be a dictionary");
				}
				IDictionary<string, object> props = tprops as IDictionary<string, object>;
				foreach (var prop in props) {
					var typ = prop.Value.GetType();
					if (typ == typeof(long)) {
						val.Set(prop.Key, (long)prop.Value);
					} else if (typ == typeof(double)) {
						val.Set(prop.Key, (double)prop.Value);
					} else if (typ == typeof(bool)) {
						val.Set(prop.Key, (bool)prop.Value);
					} else if (typ == typeof(string)) {
						val.Set(prop.Key, (string)prop.Value);
					} else {
						throw new UnknownTypeException("property " + prop.Key + " must be a integer, float, boolean, or string");
					}
				}
			}

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

		public void VisitChildren<T>(NodeChildVisitor<T> visitor) where T : INode
		{
			foreach (T item in _children[typeof(T)]) {
				visitor(item);
			}
		}

		public void Set(string key, long value)
		{
			LongConfigs[key] = value;
		}

		public void Set(string key, double value)
		{
			DoubleConfigs[key] = value;
		}

		public void Set(string key, bool value)
		{
			BoolConfigs[key] = value;
		}

		public void Set(string key, string value)
		{
			StrConfigs[key] = value;
		}

		public long GetLong(string key)
		{
			return LongConfigs[key];
		}

		public double GetDouble(string key)
		{
			return DoubleConfigs[key];
		}

		public bool GetBool(string key)
		{
			return BoolConfigs[key];
		}

		public string GetString(string key)
		{
			return StrConfigs[key];
		}
	}
}
