using FreeTypeSharp;

using System;

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
