using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class NotFoundException :Exception
    {
        public NotFoundException(string msg):base(msg){
            
        }
    }
}