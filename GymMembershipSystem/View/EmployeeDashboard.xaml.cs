using GymMembershipSystem.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GymMembershipSystem.View
{
    public partial class EmployeeDashboard : Window
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public EmployeeDashboard()
        {
            InitializeComponent();
            DataContext = new EmployeeDashboardViewModel();
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
