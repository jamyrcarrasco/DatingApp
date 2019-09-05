namespace DatingApp.APi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
    }
}