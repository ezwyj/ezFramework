using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class InfoException : Exception
    {
        public InfoException(string msg) : base(msg)
        {

        }
        public InfoException(string message, params object[] args) : this(string.Format(message, args))
        {

        }
    }
}
