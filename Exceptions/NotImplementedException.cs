using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class NotImplementedException :Exception
    {
        public NotImplementedException(string msg):base(msg)
        {
            
        }
    }
}