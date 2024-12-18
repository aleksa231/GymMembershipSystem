using GymMembershipSystem.Model;
using MySqlConnector;
using System.Windows;

public class CurrentUser
{
    static int UserID;

    public static void setUserID(string username, string password)
    {
        
        Database db = new Database();
        MySqlConnection connection = db.GetConnection();
        string query = @"
        SELECT UserID 
        FROM Users 
        WHERE UserName = @UserName AND PasswordHash = SHA2(@Password, 256)";

        MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@UserName", username);
        command.Parameters.AddWithValue("@Password", password);

        try
        {
            connection.Open();
            object result = command.ExecuteScalar();

            if (result != null)
            {
                UserID = Convert.ToInt32(result);
            }
            else
            {
                MessageBox.Show("Error geting userID");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred during login: {ex.Message}");
        }
        finally
        {
            connection.Close();
        }
    }

    public static int GetUserID()
    {
        return UserID;
    }

}