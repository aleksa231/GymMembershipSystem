
namespace GymMembershipSystem.ViewModel
{
    public class ActivityLogViewModel
    {
        public int LogID { get; set; } 
        public int UserID { get; set; } 
        public string Action { get; set; } 
        public DateTime ActionDate { get; set; } 

        public override string ToString()
        {
            return $"{ActionDate:yyyy-MM-dd HH:mm:ss} - {UserID:UserID}: {Action}";
        }
    }
}
