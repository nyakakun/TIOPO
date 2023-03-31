using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Response
    {
        public int status { get; set; }
    }

    public class ResponseAdd
    {
        public int status { get; set; }
        public int? id { get; set; }
    }
}
