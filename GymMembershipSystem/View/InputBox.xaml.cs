using System.Windows;

namespace GymMembershipSystem.View
{
    public partial class InputBox : Window
    {
        public string Prompt { get; }
        public string InputText { get; set; } = string.Empty;

        public InputBox(string prompt, string title)
        {
            InitializeComponent();
            DataContext = this;
            Prompt = prompt;
            Title = title;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
