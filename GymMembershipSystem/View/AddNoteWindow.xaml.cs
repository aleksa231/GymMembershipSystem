using GymMembershipSystem.Model;
using MySqlConnector;
using System.Windows;

namespace GymMembershipSystem.View
{
    public partial class AddNoteWindow : Window
    {
        private Member _member;

        public AddNoteWindow(Member member)
        {
            InitializeComponent();
            _member = member;
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            string noteText = NoteTextBox.Text;
            if (!string.IsNullOrEmpty(noteText))
            {
                AddNoteToDatabase(_member.MemberID, noteText);
            }
            this.Close(); 
        }

        private void AddNoteToDatabase(int memberId, string noteText)
        {
            Database db = new Database();
            MySqlConnection connection = db.GetConnection();
            string query = @"
            INSERT INTO membernotes (MemberID, Note) 
            VALUES (@MemberID, @Note)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@MemberID", memberId);
            command.Parameters.AddWithValue("@Note", noteText);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Note added successfully.");

                string logQuery = @"
                INSERT INTO ActivityLog 
                (UserID, Action, ActionDate) 
                VALUES 
                (@UserID, @Action, @ActionDate)";

                MySqlCommand logCommand = new MySqlCommand(logQuery, connection);
                logCommand.Parameters.AddWithValue("@UserID", CurrentUser.GetUserID()); 
                logCommand.Parameters.AddWithValue("@Action", $"Added note to MemberID:{memberId}");
                logCommand.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                logCommand.ExecuteNonQuery();
                Console.WriteLine("Activity logged successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding note: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
