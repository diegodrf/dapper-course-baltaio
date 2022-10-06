using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseBaltaDapper.Models
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Category> Categories { get; private set; }

        public Student()
        {
            Id = String.Empty;
            Name = String.Empty;
            Categories = new List<Category>();
        }

        public void AddCategory(Category category)
        {
            Categories.Add(category);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not null && obj is Student)
            {
                var newObj = obj as Student;
                return Id.GetHashCode() == newObj!.GetHashCode();
            }
            return false;
        }
    }
}
