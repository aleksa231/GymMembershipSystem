using System.Windows;
using GymMembershipSystem.Model;
using GymMembershipSystem.ViewModel;
using MySqlConnector;

namespace GymMembershipSystem.View
{
    public partial class AddMemberWindow : Window
    {
        private readonly EmployeeDashboardViewModel _dashboardViewModel;

        public AddMemberWindow(EmployeeDashboardViewModel dashboardViewModel)
        {
            InitializeComponent();
            _dashboardViewModel = dashboardViewModel;

            var addMemberViewModel = new AddMemberViewModel();
            DataContext = addMemberViewModel;
        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AddMemberViewModel)DataContext;

            if (viewModel.SelectedMembershipPackage == null)
            {
                MessageBox.Show("Please select a membership package.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newMember = new Member
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                JoinDate = DateTime.Now, 
                MembershipStatus = "Active",
                MembershipCardNumber = viewModel.MembershipCardNumber,
            };

            try
            {
              
                _dashboardViewModel.AddMemberToDatabase(newMember);

             
                int memberId;
                Database db = new Database();
                using (var connection = db.GetConnection())
                {
                    connection.Open();
                    string query = @"SELECT MemberID FROM Members WHERE FirstName = @FirstName AND LastName = @LastName AND Email = @Email LIMIT 1";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@FirstName", newMember.FirstName);
                    command.Parameters.AddWithValue("@LastName", newMember.LastName);
                    command.Parameters.AddWithValue("@Email", newMember.Email);

                    memberId = Convert.ToInt32(command.ExecuteScalar());
                }

                if (memberId == 0)
                {
                    MessageBox.Show("Failed to retrieve MemberID for the new member.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

           
                using (var connection = db.GetConnection())
                {
                    connection.Open();
                    string insertPaymentQuery = @"INSERT INTO MembershipPayments (MemberID, PackageID, PaymentDate, AmountPaid) 
                                         VALUES (@MemberID, @PackageID, @PaymentDate, @AmountPaid)";
                    MySqlCommand paymentCommand = new MySqlCommand(insertPaymentQuery, connection);

                    paymentCommand.Parameters.AddWithValue("@MemberID", memberId);
                    paymentCommand.Parameters.AddWithValue("@PackageID", viewModel.SelectedMembershipPackage.PackageID);
                    paymentCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                    paymentCommand.Parameters.AddWithValue("@AmountPaid", viewModel.SelectedMembershipPackage.Price);

                    paymentCommand.ExecuteNonQuery();
                }

         
                MessageBox.Show("Member added successfully, and payment recorded!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
