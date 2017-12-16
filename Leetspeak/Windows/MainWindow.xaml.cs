using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Leetspeak
{
	public partial class MainWindow : Window
	{
		private Random Random = new Random(Environment.TickCount);
		private XDocument Languages;
		private bool IsInitializing = true;
		private bool IsLeetEnabled = false;

		public MainWindow()
		{
			InitializeComponent();
			Left = Config.WindowX ?? Left;
			Top = Config.WindowY ?? Top;
			(Config.Enabled ? radEnabled : radDisabled).IsChecked = true;
			cmbHotkey.SelectedIndex = Config.Hotkey ?? cmbHotkey.SelectedIndex;

			Keyhook.Start();
			Keyhook.KeyPress += new KeyPressEventHandler(Keyhook_KeyPress);
			Keyhook.KeyUp += new KeyEventHandler(Keyhook_KeyUp);

			Languages = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Languages)));
			lstLanguages.ItemsSource = Languages.Root.Elements().Select(element => new[]
			{
				element.Attribute("Name").Value,
				Translator.TranslateText(Languages,element.Attribute("Name").Value, "The quick brown fox jumps over the lazy dog"),
			});
			for (int i = 0; i < lstLanguages.Items.Count; i++)
			{
				if ((lstLanguages.Items[i] as string[])[0] == Config.Dictionary)
				{
					lstLanguages.SelectedIndex = i;
					break;
				}
			}
			if (lstLanguages.SelectedItem == null) lstLanguages.SelectedIndex = 0;

			IsInitializing = false;
		}
		private void MainWindow_LocationChanged(object sender, EventArgs e)
		{
			if (!IsInitializing && (int)Left != -32000 && (int)Top != -32000)
			{
				Config.WindowX = (int)Left;
				Config.WindowY = (int)Top;
			}
		}

		private void lnkAbout_Click(object sender, RoutedEventArgs e)
		{
			new WindowAbout().ShowDialog();
		}
		private void radDisabled_Checked(object sender, RoutedEventArgs e)
		{
			IsLeetEnabled = false;
			if (!IsInitializing) Config.Enabled = false;
		}
		private void radEnabled_Checked(object sender, RoutedEventArgs e)
		{
			IsLeetEnabled = true;
			if (!IsInitializing) Config.Enabled = true;
		}
		private void cmbHotkey_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!IsInitializing)
			{
				Config.Hotkey = cmbHotkey.SelectedIndex;
			}
		}
		private void btnTranslate_Click(object sender, RoutedEventArgs e)
		{
			bool enabled = IsLeetEnabled;
			IsLeetEnabled = false;
			new WindowTranslate(Languages, (lstLanguages.SelectedItem as string[])[0]).ShowDialog();
			IsLeetEnabled = enabled;
		}
		private void lstLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!IsInitializing && lstLanguages.SelectedItem != null)
			{
				Config.Dictionary = (lstLanguages.SelectedItem as string[])[0];
			}
		}

		private void Keyhook_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (IsLeetEnabled)
			{
				string print = Translator.Translate(Languages, (lstLanguages.SelectedItem as string[])[0], e.KeyChar);
				if (print != e.KeyChar.ToString())
				{
					print = print
						.Replace("+", "BRCK1+BRCK2")
						.Replace("^", "BRCK1^BRCK2")
						.Replace("%", "BRCK1%BRCK2")
						.Replace("~", "BRCK1~BRCK2")
						.Replace("(", "BRCK1(BRCK2")
						.Replace(")", "BRCK1)BRCK2")
						.Replace("{", "BRCK1BRCK1BRCK2")
						.Replace("}", "BRCK1BRCK2BRCK2")
						.Replace("BRCK1", "{")
						.Replace("BRCK2", "}");

					try
					{
						SendKeys.SendWait(print);
					}
					catch { }
					e.Handled = true;
				}
			}
		}
		private void Keyhook_KeyUp(object sender, KeyEventArgs e)
		{
			if (cmbHotkey.SelectedIndex != 12 && e.KeyCode == Keys.F1 + cmbHotkey.SelectedIndex)
			{
				if (radEnabled.IsChecked.Value)
				{
					radDisabled.IsChecked = true;
				}
				else
				{
					radEnabled.IsChecked = true;
				}
			}
		}

		[DllImport("user32.dll")]
		static extern short GetAsyncKeyState(Keys vKey);
	}
}