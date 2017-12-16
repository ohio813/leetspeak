using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Leetspeak
{
	public partial class WindowTranslate : Window
	{
		private XDocument Languages;
		private bool IsInitializing = true;

		public WindowTranslate(XDocument languages, string currentLanguage)
		{
			Languages = languages;

			InitializeComponent();
			Width = Config.TranslateWindowWidth ?? Width;
			Height = Config.TranslateWindowHeight ?? Height;

			cmbLanguage.ItemsSource = languages.Root.Elements().Select(element => element.Attribute("Name").Value);
			cmbLanguage.SelectedValue = currentLanguage;

			IsInitializing = false;
		}
		private void WindowTranslate_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!IsInitializing)
			{
				Config.TranslateWindowWidth = (int)Width;
				Config.TranslateWindowHeight = (int)Height;
			}
		}

		private void btnTranslate_Click(object sender, RoutedEventArgs e)
		{
			txtTranslatedText.Text = Translator.TranslateText(Languages, cmbLanguage.SelectedValue.ToString(), txtText.Text);
		}
	}
}