using GymMembershipSystem.ViewModel;
using System.Windows;

namespace GymMembershipSystem.View
{

    public partial class ActivityLog : Window
    {
        public ActivityLog()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
    }
}
