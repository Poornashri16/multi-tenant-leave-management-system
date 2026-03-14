public class ProfileResponseDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string Role { get; set; }
}

public class UpdateProfileDto
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
}