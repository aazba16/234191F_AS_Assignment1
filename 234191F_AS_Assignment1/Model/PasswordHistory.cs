namespace _234191F_AS_Assignment1.Model
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Foreign key to the user
        public string HashedPassword { get; set; }  // Store the hashed password
        public DateTime DateChanged { get; set; }  // Date when the password was changed
    }
}
