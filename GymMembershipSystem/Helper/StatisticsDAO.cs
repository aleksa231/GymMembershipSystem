using MySqlConnector;
using GymMembershipSystem.Model;


public class StatisticsDAO
{
    Database db = new Database();

    public MembershipStatistics GetStatistics()
    {
        var statistics = new MembershipStatistics();

        using (var connection = db.GetConnection())
        {
            try
            {
                connection.Open();

                using (var totalRevenueCommand = new MySqlCommand("SELECT SUM(AmountPaid) AS TotalRevenue FROM MembershipPayments", connection))
                {
                    var result = totalRevenueCommand.ExecuteScalar();
                    statistics.TotalRevenue = result == DBNull.Value || result == null ? 0 : Convert.ToDecimal(result);
                }

                using (var popularPackageCommand = new MySqlCommand(
                    @"SELECT mp.PackageName, COUNT(*) AS PackageCount 
                      FROM MembershipPayments AS p 
                      JOIN MembershipPackages AS mp ON p.PackageID = mp.PackageID 
                      GROUP BY mp.PackageName 
                      ORDER BY PackageCount DESC 
                      LIMIT 1", connection))
                {
                    using (var reader = popularPackageCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statistics.MostPopularPackage = reader.IsDBNull(0) ? "None" : reader.GetString("PackageName");
                            statistics.PackageCount = reader.IsDBNull(1) ? 0 : reader.GetInt32("PackageCount");
                        }
                        else
                        {
                            statistics.MostPopularPackage = "None";
                            statistics.PackageCount = 0;
                        }
                    }
                }

                using (var paymentsThisMonthCommand = new MySqlCommand(
                    @"SELECT COUNT(*) AS PaymentsThisMonth 
                      FROM MembershipPayments 
                      WHERE MONTH(PaymentDate) = MONTH(CURDATE()) 
                        AND YEAR(PaymentDate) = YEAR(CURDATE())", connection))
                {
                    var result = paymentsThisMonthCommand.ExecuteScalar();
                    statistics.PaymentsThisMonth = result == DBNull.Value || result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching statistics: " + ex.Message);
            }
        }

        return statistics;
    }
}
