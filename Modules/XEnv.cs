using System.Collections.Generic;
using System.IO;

namespace MapEditor.Modules
{
	public class XEnv
	{
		Dictionary<string, string> s_key = new Dictionary<string, string>();

		public static void CreateInstance()
		{
			if (Instance == null)
			{
				Instance = new XEnv();
			}
		}

		public bool LoadFromFile(string pszFileName)
		{
			if (!File.Exists(pszFileName)) return false;

			StreamReader file = new StreamReader(pszFileName);
			string line;

			while ((line = file.ReadLine()) != null)
			{
				line.Trim();
				string[] array = line.Split(new char[] { ':' }, 2);
				if (!line.StartsWith("##") && array.Length == 2)
				{
					string s1 = array[0].ToLower().Trim();
					string s2 = array[1].Trim();
					s_key.Add(s1, s2);
				}
			}
			return true;
		}

		public bool GetValue(string key, bool szdefault)
		{
			if (s_key.ContainsKey(key))
			{
				bool value;
				bool.TryParse(s_key[key], out value);
				return value;
			}
			return szdefault;
		}

		public int GetValue(string key, int szdefault)
		{
			if (s_key.ContainsKey(key))
			{
				int value;
				int.TryParse(s_key[key], out value);
				return value;
			}
			return szdefault;
		}

		public decimal GetValue(string key, decimal szdefault)
		{
			if (s_key.ContainsKey(key))
			{
				decimal value;
				decimal.TryParse(s_key[key], out value);
				return value;
			}
			return szdefault;
		}

		public string GetValue(string key, string szdefault)
		{
			if (s_key.ContainsKey(key))
			{
				return s_key[key];
			}
			return szdefault;
		}

		public string[] GetValue(string key, char separator, string[] szdefault)
		{
			if (s_key.ContainsKey(key))
			{
				return s_key[key].Split(separator);
			}
			return szdefault;
		}

		public static XEnv Instance = null;
	}
}
