using Dapper.Contrib.Extensions;

namespace CourseProject.Models
{
    [Table("[Role]")]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{nameof(Role)}({Id},{Name},{Slug})";
        }
    }
}
