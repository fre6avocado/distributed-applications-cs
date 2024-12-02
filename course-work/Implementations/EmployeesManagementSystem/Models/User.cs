using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Guest = 0,
    User = 1,
    Admin = 2
}

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; }

    [Required, MaxLength(200)]
    public string Password { get; set; } // Ще използваме хеширане

    public UserRole Role { get; set; }
}
