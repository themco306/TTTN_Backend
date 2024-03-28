using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class UnauthorizedAccessException :Exception
    {
        public UnauthorizedAccessException(string msg):base(msg)
        {
            
        }
    }
}