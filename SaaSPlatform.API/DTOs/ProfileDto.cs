public class ProfileResponseDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime DateOfJoining { get; set; } 
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
}