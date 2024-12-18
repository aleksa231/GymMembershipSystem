using GymMembershipSystem.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySqlConnector;
using GymMembershipSystem.View;

namespace GymMembershipSystem
{
    public partial class MainWindow : Window
    {

        EmployeeDashboard ed;
        AdminDashboard ad;


        private string currentTheme = "LightTheme";

        public MainWindow()
        {
            InitializeComponent();
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
            WindowState = WindowState.Normal;
            Height = 370;
            Width = 300;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool AuthenticateUser(string username, string password)
        {
            Database db = new Database();
            MySqlConnection connection = db.GetConnection();
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:" + ex.Message);
            }

            var command = new MySqlCommand("SELECT PasswordHash FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            var result = command.ExecuteScalar();

            if (result != null)
            {
                string storedPasswordHash = result.ToString();
                return (storedPasswordHash == PasswordHasher.HashPassword(password));
            }
            else
            {
                MessageBox.Show("Wrong username or password.");
                return false;
            }

        }

        private void LoginBtn_click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(username, password))
            {
                CurrentUser.setUserID(username, password);
                string savedTheme = GetSavedThemeForUser(CurrentUser.GetUserID().ToString());

                if (!string.IsNullOrEmpty(savedTheme))
                {
                    currentTheme = savedTheme;  
                }
                else
                {
                    currentTheme = "LightTheme"; 
                }

                ((App)Application.Current).ApplyTheme(currentTheme);

                if (UserService.IsUserAdmin(username))
                {
                    ad = new AdminDashboard();
                    ad.Show();
                }
                else
                {
                    ed = new EmployeeDashboard();
                    ed.Show();
                }
            }
        }

        private string GetSavedThemeForUser(string userID)
        {
            string query = "SELECT PreferredTheme FROM configurations WHERE UserID = @UserID";

            using (var connection = new Database().GetConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserID", userID);
                connection.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return result.ToString(); 
                }
                else
                {
                    return null;  
                }
            }
        }

        private void OnToggleThemeClick(object sender, RoutedEventArgs e)
        {
            switch (currentTheme)
            {
                case "LightTheme":
                    currentTheme = "DarkTheme";
                    break;
                case "DarkTheme":
                    currentTheme = "ColorfulTheme";
                    break;
                default:
                    currentTheme = "LightTheme";
                    break;
            }

            ((App)Application.Current).ApplyTheme(currentTheme);

            if (CurrentUser.GetUserID() > 0)  
            {
                SaveUserTheme(CurrentUser.GetUserID().ToString(), currentTheme);
            }
        }

        private void SaveUserTheme(string userID, string theme)
        {
            string checkQuery = "SELECT ConfigID FROM configurations WHERE UserID = @UserID";

            using (var connection = new Database().GetConnection())
            {
                MySqlCommand cmd = new MySqlCommand(checkQuery, connection);
                cmd.Parameters.AddWithValue("@UserID", userID);
                connection.Open();

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string updateQuery = "UPDATE configurations SET PreferredTheme = @PreferredTheme WHERE UserID = @UserID";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                    updateCmd.Parameters.AddWithValue("@PreferredTheme", theme);
                    updateCmd.Parameters.AddWithValue("@UserID", userID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = "INSERT INTO configurations (UserID, PreferredTheme) VALUES (@UserID, @PreferredTheme)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@UserID", userID);
                    insertCmd.Parameters.AddWithValue("@PreferredTheme", theme);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}