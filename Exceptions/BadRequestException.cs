using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string msg):base(msg){
            
        }

    }
}