using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Leetspeak
{
	public static class Translator
	{
		private static Random Random = new Random(Environment.TickCount);

		public static string Translate(XDocument languages, string language, char keyChar)
		{
			try
			{
				XElement translation = languages.Root.Elements()
					.Where(element => element.Attribute("Name").Value == language).Elements()
					.Where(element => element.Attribute("English").Value == keyChar.ToString().ToLower())
					.FirstOrDefault();

				if (translation == null)
				{
					return keyChar.ToString();
				}

				string[] characters;
				if (translation.Attribute("Leet") != null)
				{
					characters = translation.Attribute("Leet").Value.Split(',');
				}
				else if (char.IsLower(keyChar) && translation.Attribute("LeetLower") != null)
				{
					characters = translation.Attribute("LeetLower").Value.Split(',');
				}
				else if (char.IsUpper(keyChar) && translation.Attribute("LeetUpper") != null)
				{
					characters = translation.Attribute("LeetUpper").Value.Split(',');
				}
				else
				{
					return "";
				}
				return characters[Random.Next(characters.Length)];
			}
			catch
			{
				return keyChar.ToString();
			}
		}
		public static string TranslateText(XDocument languages, string language, string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char ch in text) stringBuilder.Append(Translate(languages, language, ch));
			return stringBuilder.ToString();
		}
	}
}