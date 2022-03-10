using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Utils
{
    class ByteArray
    {
        byte[] Bytes;
        int Length;
        public ByteArray(byte[] inputBytes)
        {
            this.Bytes = inputBytes;
            this.Length = inputBytes.Length;
        }
        public ByteArray()
        {

        }
        public void SetBytes(byte[] inputBytes)
        {
            this.Bytes = inputBytes;
            this.Length = inputBytes.Length;
        }
        public ByteArray Sum(ByteArray bytesB)
        {
            byte[] a = Bytes;
            byte[] b = bytesB.Bytes;
            byte[] ret = new byte[Math.Max(a.Length,b.Length)];
            List<byte> result = new List<byte>();
            int carry = 0;
            for (int pos = 0; pos < ret.Length; ++pos)
            {
                int sum = a[pos] + b[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            if (carry > 0)
            {
                Array.Resize<byte>(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = (byte)(carry);
            }
            return new ByteArray(ret);
        }
        static public ByteArray Sum(byte[] a, byte[] b)
        {
            byte[] ret = new byte[Math.Max(a.Length, b.Length)];
            List<byte> result = new List<byte>();
            int carry = 0;
            for (int pos = 0; pos < ret.Length; ++pos)
            {
                int sum = a[pos] + b[pos] + carry;
                ret[pos] = (byte)(sum & 0xFF);
                carry = sum >> 8;
            }
            if (carry > 0)
            {
                Array.Resize<byte>(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = (byte)(carry);
            }
            return new ByteArray(ret);
        }
        public void PrettyPrint()
        {
            Console.Write("[");
            foreach (byte item in Bytes)
            {
                Console.Write(item+" ");
            }
            Console.WriteLine("]");
        }
    }
}
