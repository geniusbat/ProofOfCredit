using ProofOfCredit.Utils;
using System;

namespace ProofOfCredit
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] a = BitConverter.GetBytes(253*10000);
            ByteArray ab = new ByteArray(a);
            ByteArray bb = new ByteArray(BitConverter.GetBytes(5*10));
            ByteArray cb = ab.Sum(bb);
            ab.PrettyPrint();
            bb.PrettyPrint();
            cb.PrettyPrint();
        }
    }
}
