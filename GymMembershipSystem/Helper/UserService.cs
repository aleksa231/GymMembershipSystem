using GymMembershipSystem.Model;
using MySqlConnector;

public class UserService
{
    public static bool IsUserAdmin(string username)
    {
        bool isAdmin = false;

        Database db = new Database();

        using (MySqlConnection connection = db.GetConnection())
        {
            string query = @"
                SELECT r.RoleName
                FROM users u
                INNER JOIN roles r ON u.RoleID = r.RoleID
                WHERE u.Username = @Username";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result.ToString() == "Administrator")
                {
                    isAdmin = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if user is admin: {ex.Message}");
            }
        }

        return isAdmin;
    }
}
