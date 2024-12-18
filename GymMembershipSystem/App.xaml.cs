using System.Windows;

namespace GymMembershipSystem
{

    public partial class App : Application
    {
        public void ApplyTheme(string theme)
        {
            string themePath = $"Themes/{theme}.xaml";

            Application.Current.Resources.MergedDictionaries.Clear();

            var themeDictionary = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(themeDictionary);

        }
    }

}
