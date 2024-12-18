using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GymMembershipSystem.Model;
using MySqlConnector;

namespace GymMembershipSystem.ViewModel
{
    public class AddMemberViewModel : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private int _membershipTypeID;
        private string _membershipStatus;
        private string _membershipCardNumber;

        public AddMemberViewModel()
        {
            LoadMembershipPackages();
        }

        private ObservableCollection<MembershipPackage> membershipPackages;
        public ObservableCollection<MembershipPackage> MembershipPackages
        {
            get => membershipPackages;
            set
            {
                membershipPackages = value;
                OnPropertyChanged();
            }
        }

        private MembershipPackage selectedMembershipPackage;
        public MembershipPackage SelectedMembershipPackage
        {
            get => selectedMembershipPackage;
            set
            {
                selectedMembershipPackage = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public int MembershipTypeID
        {
            get => _membershipTypeID;
            set
            {
                _membershipTypeID = value;
                OnPropertyChanged();
            }
        }

        public string MembershipStatus
        {
            get => _membershipStatus;
            set
            {
                _membershipStatus = value;
                OnPropertyChanged();
            }
        }

        public string MembershipCardNumber
        {
            get => _membershipCardNumber;
            set
            {
                _membershipCardNumber = value;
                OnPropertyChanged();
            }
        }
        private void LoadMembershipPackages()
        {
            MembershipPackages = new ObservableCollection<MembershipPackage>();

            Database db = new Database();

            using (MySqlConnection connection = db.GetConnection()) 
            {
                string query = "SELECT PackageID, PackageName, Price, DurationMonths FROM membershippackages";

                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MembershipPackages.Add(new MembershipPackage
                            {
                                PackageID = reader.GetInt32("PackageID"),
                                PackageName = reader.GetString("PackageName"),
                                Price = reader.GetDecimal("Price"),
                                Duration = reader.GetInt32("DurationMonths")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading membership packages: {ex.Message}");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MembershipPackage
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } 
    }
}
