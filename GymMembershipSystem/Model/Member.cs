public class Member
{
    public int MemberID { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime JoinDate { get; set; }
    public string MembershipStatus { get; set; }
    public string MembershipCardNumber { get; set; }
    public string Name { get; set; }

    public Member(int memberId = 0, string firstName = "", string lastName = "", string email = "",
                  string phoneNumber = "", DateTime? joinDate = null, string membershipStatus = "Active", string membershipCardNumber = "")
    {
        MemberID = memberId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        JoinDate = joinDate ?? DateTime.Now;
        MembershipStatus = membershipStatus;
        MembershipCardNumber = membershipCardNumber;
        Name = $"{firstName} {lastName}";
    }
}
