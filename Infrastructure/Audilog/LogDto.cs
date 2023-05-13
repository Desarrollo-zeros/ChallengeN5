using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Audilog
{
    public class LogDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Operation { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

    }
}
