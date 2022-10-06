using Dapper.Contrib.Extensions;

namespace CourseProject.Models
{
    [Table("[User]")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        [Write(false)]
        public List<Role> Roles { get; set; }

        public User()
        {
            Name = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            Bio = string.Empty;
            Image = string.Empty;
            Slug = string.Empty;
            Roles = new List<Role>();
        }

        public override string ToString()
        {
            return $"{nameof(User)}({Id},{Name},{Slug})";
        }
    }
}
