using Microsoft.Win32;

namespace Leetspeak
{
	public static class Config
	{
		private static RegistryKey Key
		{
			get
			{
				return Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("Leetspeak2");
			}
		}

		public static int? WindowX
		{
			get
			{
				return (int?)Key.GetValue("WindowX");
			}
			set
			{
				Key.SetValue("WindowX", value);
			}
		}
		public static int? WindowY
		{
			get
			{
				return (int?)Key.GetValue("WindowY");
			}
			set
			{
				Key.SetValue("WindowY", value);
			}
		}
		public static bool Enabled
		{
			get
			{
				return (int)Key.GetValue("Enabled", 1) == 1;
			}
			set
			{
				Key.SetValue("Enabled", value ? 1 : 0);
			}
		}
		public static int? Hotkey
		{
			get
			{
				return (int?)Key.GetValue("Hotkey");
			}
			set
			{
				Key.SetValue("Hotkey", value);
			}
		}
		public static string Dictionary
		{
			get
			{
				return (string)Key.GetValue("Dictionary");
			}
			set
			{
				Key.SetValue("Dictionary", value);
			}
		}
		public static int? TranslateWindowWidth
		{
			get
			{
				return (int?)Key.GetValue("TranslateWindowWidth");
			}
			set
			{
				Key.SetValue("TranslateWindowWidth", value);
			}
		}
		public static int? TranslateWindowHeight
		{
			get
			{
				return (int?)Key.GetValue("TranslateWindowHeight");
			}
			set
			{
				Key.SetValue("TranslateWindowHeight", value);
			}
		}
	}
}