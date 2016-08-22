using System;
using System.Runtime.InteropServices;

namespace LibUdt
{
    public class UdtException : Exception
    {
        public int ErrorCode { get; private set; }

        public UdtException() : base(GetErrorDesc())
        {
            this.ErrorCode = UDT.GetLastErrorCode();
        }

        public UdtException(string msg) : base(msg)
        {
            this.ErrorCode = -1;
        }

        static string GetErrorDesc()
        {
            IntPtr p = UDT.GetLastErrorDesc();
            string str = Marshal.PtrToStringAnsi(p);

            return str;
        }
    }
}
