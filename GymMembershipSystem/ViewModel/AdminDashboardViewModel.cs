using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GymMembershipSystem.Model;
using GymMembershipSystem.View;
using MySqlConnector;

namespace GymMembershipSystem.ViewModel
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        public UserViewModel NewUser { get; set; }
        public MembershipPackageViewModel NewPackage { get; set; }

        public ObservableCollection<ActivityLogViewModel> ActivityLogs { get; set; }

        public ICommand AddUserCommand { get; }
        public ICommand AddPackageCommand { get; }
        public ICommand RemoveUserCommand { get; }
        public ICommand RemovePackageCommand { get; }
        public ICommand ViewActivityLogCommand { get; }
        public ICommand ViewStatisticsCommand { get; }

        public AdminDashboardViewModel()
        {
            NewUser = new UserViewModel();
            NewPackage = new MembershipPackageViewModel();
            ActivityLogs = new ObservableCollection<ActivityLogViewModel>();

            AddUserCommand = new RelayCommand(AddUser, CanExecuteAddUser);
            AddPackageCommand = new RelayCommand(AddPackage, CanExecuteAddPackage);
            RemoveUserCommand = new RelayCommand(RemoveUser);
            RemovePackageCommand = new RelayCommand(RemovePackage);
            ViewActivityLogCommand = new RelayCommand(ViewActivityLog);
            ViewStatisticsCommand = new RelayCommand(OpenStatisticsView);

        }

        private void OpenStatisticsView()
        {
            StatisticsDAO statisticsDAO = new StatisticsDAO();
            MembershipStatistics statistics = statisticsDAO.GetStatistics();
            MessageBox.Show(
            $"Total Revenue: ${statistics.TotalRevenue}\n" +
            $"Most Popular Package: {statistics.MostPopularPackage} ({statistics.PackageCount} sales)\n" +
            $"Payments This Month: {statistics.PaymentsThisMonth}",
            "Membership Statistics");
        }
        private void ViewActivityLog(object parameter = null)
        {
            LoadActivityLogsFromDatabase();
        }

        private void LoadActivityLogsFromDatabase()
        {
            Database db = new Database();
            string query = "SELECT * FROM activitylog ORDER BY ActionDate DESC";
            using (var connection = db.GetConnection())
            {
                var command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        ActivityLogs.Clear();
                        while (reader.Read())
                        {
                            ActivityLogs.Add(new ActivityLogViewModel
                            {
                                LogID = reader.GetInt32("LogID"),
                                UserID = reader.GetInt32("UserID"),
                                Action = reader.GetString("Action"),
                                ActionDate = reader.GetDateTime("ActionDate")
                            });
                        }
                    }

                    var activityLogWindow = new ActivityLog
                    {
                        DataContext = this
                    };
                    activityLogWindow.Show();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void RemoveUser()
        {
            var username = GetInputFromUser("Enter the username to remove:", "Remove User");
            if (username == null) return;

            RemoveUserFromDatabase(username);
        }

        private void RemoveUserFromDatabase(string username)
        {
            Database db = new Database();
            string query = "DELETE FROM Users WHERE Username = @Username";
            using (var connection = db.GetConnection())
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "User removed successfully." : "User not found.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void RemovePackage()
        {
            var packageName = GetInputFromUser("Enter the package name to remove:", "Remove Package");
            if (packageName == null) return;

            RemovePackageFromDatabase(packageName);
        }

        private void RemovePackageFromDatabase(string packageName)
        {
            Database db = new Database();
            string query = "DELETE FROM MembershipPackages WHERE PackageName = @PackageName";
            using (var connection = db.GetConnection())
            {
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@PackageName", packageName);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "Package removed successfully." : "Package not found.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private string? GetInputFromUser(string prompt, string title)
        {
            InputBox inputBox = new InputBox(prompt, title);
            bool isConfirmed = inputBox.ShowDialog() ?? false;

            return isConfirmed ? inputBox.InputText : null;
        }

        private bool CanExecuteAddUser(object parameter) =>
            !string.IsNullOrWhiteSpace(NewUser.Username) &&
            !string.IsNullOrWhiteSpace(NewUser.Role);

        private void AddUser(object parameter)
        {
            string roleId = NewUser.Role == "Admin" ? "1" : "2";
            Database db = new Database();
            using (var connection = db.GetConnection())
            {
                string query = "INSERT INTO Users (Username, PasswordHash, RoleID) VALUES (@Username, @PasswordHash, @RoleID)";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", NewUser.Username);
                command.Parameters.AddWithValue("@PasswordHash", PasswordHasher.HashPassword(NewUser.PasswordHash));
                command.Parameters.AddWithValue("@RoleID", roleId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    ClearUser();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void ClearUser()
        {
            NewUser.Username = string.Empty;
            NewUser.PasswordHash = string.Empty;
            NewUser.Role = null;
        }

        private bool CanExecuteAddPackage(object parameter) =>
            !string.IsNullOrWhiteSpace(NewPackage.PackageName) &&
            NewPackage.DurationMonths > 0 &&
            NewPackage.Price > 0;

        private void AddPackage(object parameter)
        {
            Database db = new Database();
            using (var connection = db.GetConnection())
            {
                string query = "INSERT INTO MembershipPackages (PackageName, DurationMonths, Price) VALUES (@PackageName, @DurationMonths, @Price)";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@PackageName", NewPackage.PackageName);
                command.Parameters.AddWithValue("@DurationMonths", NewPackage.DurationMonths);
                command.Parameters.AddWithValue("@Price", NewPackage.Price);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    ClearPackage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void ClearPackage()
        {
            NewPackage.PackageName = string.Empty;
            NewPackage.DurationMonths = null;
            NewPackage.Price = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
