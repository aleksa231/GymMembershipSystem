using GymMembershipSystem.Model;
using GymMembershipSystem.ViewModel;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace GymMembershipSystem.View
{

    public partial class AddPaymentDialog : Window
    {

        Member SelectedMember;
        public AddPaymentDialog(Member selectedMember)
        {
            InitializeComponent();
            
            DataContext = this;

            SelectedMember = selectedMember;    

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

        private void addPackage_Click(object sender, RoutedEventArgs e)
        {
            Database db = new Database();

            using (var connection = db.GetConnection())
            {
                connection.Open();

                string insertPaymentQuery = @"INSERT INTO MembershipPayments (MemberID, PackageID, PaymentDate, AmountPaid) 
                                      VALUES (@MemberID, @PackageID, @PaymentDate, @AmountPaid)";
                MySqlCommand paymentCommand = new MySqlCommand(insertPaymentQuery, connection);

                paymentCommand.Parameters.AddWithValue("@MemberID", SelectedMember.MemberID);
                paymentCommand.Parameters.AddWithValue("@PackageID", this.SelectedMembershipPackage.PackageID);
                paymentCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                paymentCommand.Parameters.AddWithValue("@AmountPaid", this.SelectedMembershipPackage.Price);

                paymentCommand.ExecuteNonQuery();

                string updateStatusQuery = @"UPDATE members 
                                     SET MembershipStatus = 'Active' 
                                     WHERE FirstName = @FirstName 
                                       AND LastName = @LastName 
                                       AND Email = @Email";
                MySqlCommand updateStatusCommand = new MySqlCommand(updateStatusQuery, connection);

                updateStatusCommand.Parameters.AddWithValue("@FirstName", SelectedMember.FirstName);
                updateStatusCommand.Parameters.AddWithValue("@LastName", SelectedMember.LastName);
                updateStatusCommand.Parameters.AddWithValue("@Email", SelectedMember.Email);

                updateStatusCommand.ExecuteNonQuery();
            }

            MessageBox.Show("Package added and member status updated to Active.");
        }

    }
}
