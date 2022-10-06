using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseBaltaDapper.Models
{
    public class CategoryStudent
    {
        public Category Category { get; set; }
        public Student Student { get; set; }
    }
}
