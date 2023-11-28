using System;

namespace SISMA.Core.Models
{
    public class SismaMappingException : Exception
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public SismaMappingException(string code, string message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
    }
}
