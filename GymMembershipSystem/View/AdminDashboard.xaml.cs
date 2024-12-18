using GymMembershipSystem.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GymMembershipSystem.View
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminDashboardViewModel viewModel)
            {
                viewModel.NewUser.PasswordHash = ((PasswordBox)sender).Password;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
