using GymMembershipSystem.Model;
using MySqlConnector;
using System.Windows;


namespace GymMembershipSystem.View
{
    public partial class ReadNotesWindow : Window
    {
        private Member _member;

        public ReadNotesWindow(Member member)
        {
            InitializeComponent();
            _member = member;
            LoadNotes();
        }

        private void LoadNotes()
        {
            Database db = new Database();
            MySqlConnection connection = db.GetConnection();
            string query = @"
            SELECT Note, CreatedDate 
            FROM membernotes 
            WHERE MemberID = @MemberID 
            ORDER BY CreatedDate DESC";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@MemberID", _member.MemberID);

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string note = reader["Note"].ToString();
                    DateTime createdDate = Convert.ToDateTime(reader["CreatedDate"]);
                    NotesListBox.Items.Add($"{createdDate:yyyy-MM-dd HH:mm:ss} - {note}");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notes: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
