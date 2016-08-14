using System;

namespace LibUdt
{
    public class UdtException : Exception
    {
        public int ErrorCode { get; private set; }

        public UdtException() : base(UDT.GetLastErrorDesc())
        {
            this.ErrorCode = UDT.GetLastErrorCode();
        }

        public UdtException(string msg) : base(msg)
        {
        }
    }
}
