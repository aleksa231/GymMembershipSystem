using GymMembershipSystem.Model;
using System.Collections.ObjectModel;
using MySqlConnector;
using GymMembershipSystem.View;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace GymMembershipSystem.ViewModel
{
    public class EmployeeDashboardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Member> _allMembers; 
        public ObservableCollection<Member> AllMembers
        {
            get => _allMembers;
            set
            {
                _allMembers = value;
                OnPropertyChanged();
            }
        }


        private ICollectionView _membersView;
        public ICollectionView MembersView
        {
            get => _membersView;
            private set
            {
                _membersView = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddMemberCommand { get; set; }
        public RelayCommand RemoveMemberCommand { get; set; }
        public RelayCommand LoadMembersCommand { get; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand AddNoteCommand { get; set; }
        public Member SelectedMember { get; set; }
        public RelayCommand ReadNotesCommand { get; set; }
        public RelayCommand AddPaymentCommand { get; }



        private string _searchText;

        public EmployeeDashboardViewModel()
        {
            AllMembers = new ObservableCollection<Member>();
            MembersView = CollectionViewSource.GetDefaultView(AllMembers);
            MembersView.Filter = FilterMembers;
            ReadNotesCommand = new RelayCommand(ReadNotes);
            AddMemberCommand = new RelayCommand(parameter => AddMember(parameter));
            RemoveMemberCommand = new RelayCommand(parameter => RemoveMember(parameter), CanRemoveMember);
            SearchCommand = new RelayCommand(Search);
            AddNoteCommand = new RelayCommand(AddNote);
            LoadMembersCommand = new RelayCommand(parameter => LoadMembers());
            AddPaymentCommand = new RelayCommand(parameter => AddPayment(parameter), CanAddPayment);

            LoadMembers();
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    MembersView.Refresh(); 
                }
            }
        }

        private void AddPayment(object parameter)
        {
            if (SelectedMember != null)
            {
                var addPaymentDialog = new AddPaymentDialog(SelectedMember);
                addPaymentDialog.ShowDialog();
            }
        }

        private bool CanAddPayment(object parameter)
        {
            if (SelectedMember != null)
            {
                return SelectedMember.MembershipStatus == "Expired";
            }
            return false;
        }
    
        private void AddMember(object parameter)
        {
            new AddMemberWindow(this).Show();
        }

        private void AddNote(object parameter)
        {
            Member member = parameter as Member;
            if (member != null)
            {
                var addNoteWindow = new AddNoteWindow(member);
                addNoteWindow.ShowDialog();
            }
        }
        private void ReadNotes(object parameter)
        {
            Member member = parameter as Member; 
            if (member != null)
            {
                var readNotesWindow = new ReadNotesWindow(member);
                readNotesWindow.ShowDialog();
            }
        }


        public void AddMemberToDatabase(Member newMember)
        {
            Database db = new Database();
            MySqlConnection connection = db.GetConnection();

            using (connection)
            {
                string memberQuery = @"
                INSERT INTO Members 
                (FirstName, LastName, Email, PhoneNumber, JoinDate, MembershipStatus, MembershipCardNumber) 
                VALUES 
                (@FirstName, @LastName, @Email, @PhoneNumber, @JoinDate, @MembershipStatus, @MembershipCardNumber)";

                MySqlCommand memberCommand = new MySqlCommand(memberQuery, connection);
                memberCommand.Parameters.AddWithValue("@FirstName", newMember.FirstName);
                memberCommand.Parameters.AddWithValue("@LastName", newMember.LastName);
                memberCommand.Parameters.AddWithValue("@Email", newMember.Email);
                memberCommand.Parameters.AddWithValue("@PhoneNumber", newMember.PhoneNumber);
                memberCommand.Parameters.AddWithValue("@JoinDate", newMember.JoinDate);
                memberCommand.Parameters.AddWithValue("@MembershipStatus", newMember.MembershipStatus);
                memberCommand.Parameters.AddWithValue("@MembershipCardNumber", newMember.MembershipCardNumber);

                try
                {
                    connection.Open();
                    memberCommand.ExecuteNonQuery();

                    Console.WriteLine("Member added successfully.");

                    string logQuery = @"
                    INSERT INTO ActivityLog 
                    (UserID, Action, ActionDate) 
                    VALUES 
                    (@UserID, @Action, @ActionDate)";

                    MySqlCommand logCommand = new MySqlCommand(logQuery, connection);
                    logCommand.Parameters.AddWithValue("@UserID", CurrentUser.GetUserID());
                    logCommand.Parameters.AddWithValue("@Action", $"Added new member: {newMember.FirstName} {newMember.LastName}");
                    logCommand.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                    logCommand.ExecuteNonQuery();
                    Console.WriteLine("Activity logged successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while adding member or logging activity: {ex.Message}");
                }
            }

            // Add the new member to the in-memory list
            AllMembers.Add(newMember);
        }


        private void RemoveMember(object parameter)
        {
            if (SelectedMember != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(SelectedMember.FirstName) ||
                        string.IsNullOrWhiteSpace(SelectedMember.LastName) ||
                        string.IsNullOrWhiteSpace(SelectedMember.Email))
                    {
                        MessageBox.Show("Selected member's details are incomplete. Cannot proceed with deletion.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Database db = new Database();
                    MySqlConnection connection = db.GetConnection();

                    using (connection)
                    {
                        connection.Open();

                        string query = @"
                        DELETE FROM Members
                        WHERE FirstName = @FirstName
                        AND LastName = @LastName
                        AND Email = @Email";

                        MySqlCommand command = new MySqlCommand(query, connection);

                        using (command)
                        {
                            command.Parameters.AddWithValue("@FirstName", SelectedMember.FirstName);
                            command.Parameters.AddWithValue("@LastName", SelectedMember.LastName);
                            command.Parameters.AddWithValue("@Email", SelectedMember.Email);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("No matching member found in the database to delete.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Member successfully removed from the database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                string logQuery = @"
                                INSERT INTO ActivityLog 
                                (UserID, Action, ActionDate) 
                                VALUES 
                                (@UserID, @Action, @ActionDate)";

                                MySqlCommand logCommand = new MySqlCommand(logQuery, connection);
                                logCommand.Parameters.AddWithValue("@UserID", CurrentUser.GetUserID()); // Replace with actual user ID from your application context
                                logCommand.Parameters.AddWithValue("@Action", $"Removed member: {SelectedMember.FirstName} {SelectedMember.LastName}");
                                logCommand.Parameters.AddWithValue("@ActionDate", DateTime.Now);

                                logCommand.ExecuteNonQuery();
                                Console.WriteLine("Activity logged successfully.");
                            }
                        }
                    }

                    AllMembers.Remove(SelectedMember);
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show($"A null reference occurred: {ex.Message}\nPlease ensure that all member details are properly set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while removing the member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a member to remove.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }




        private bool CanRemoveMember(object parameter)
        {
            return SelectedMember != null;
        }

        public void LoadMembers()
        {
            AllMembers.Clear();

            Database db = new Database();
            MySqlConnection connection = db.GetConnection();

            using (connection)
            {
                string query = @"
                SELECT MemberID, FirstName, LastName, PhoneNumber, Email, MembershipCardNumber, JoinDate, MembershipStatus
                FROM Members";

                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Member member = new Member
                            {
                                MemberID = reader.GetInt32("MemberID"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                Email = reader.GetString("Email"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                JoinDate = reader.GetDateTime("JoinDate"),
                                MembershipStatus = reader.GetString("MembershipStatus"),
                                MembershipCardNumber = reader.GetString("MembershipCardNumber"),
                                Name = $"{reader.GetString("FirstName")} {reader.GetString("LastName")}"
                            };

                            AllMembers.Add(member);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private bool FilterMembers(object item)
        {
            if (item is Member member)
            {
                return string.IsNullOrEmpty(SearchText) ||
                       member.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       member.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       member.PhoneNumber.Contains(SearchText);
            }
            return false;
        }

        private void Search(object parameter)
        {
            MembersView.Refresh();
        }

        public void NotifyCanExecuteChanged()
        {
            RemoveMemberCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
