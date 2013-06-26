using System;
using SimpleJson;
using System.Runtime.Serialization;
using System.IO;

namespace Stucco
{
	public class Loader
	{
		string filename;
		JsonObject fileData;

		public Loader(string jsonFilename)
		{
			filename = jsonFilename;
		}

		public void Begin()
		{
			if (fileData == null) {
				var txt = File.ReadAllText(filename);
				object obj = SimpleJson.SimpleJson.DeserializeObject(txt);
				if (!(obj is JsonObject)) {
					throw new SerializationException("Loader expects the top-level json primative to be an object");
				}
				fileData = obj as JsonObject;
			}
		}
	}
}

