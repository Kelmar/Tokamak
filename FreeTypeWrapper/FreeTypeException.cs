using FreeTypeSharp.Native;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeTypeWrapper
{
    public class FreeTypeException : Exception
    {
        public FreeTypeException(FT_Error errorCode)
        {
            ErrorCode = errorCode;
        }

        public FT_Error ErrorCode { get; set; }
    }
}
