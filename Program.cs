using ProofOfCredit.Members;
using ProofOfCredit.NaughtyList;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;

namespace ProofOfCredit
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockViolationEntry e = new BlockViolationEntry();
            Console.WriteLine(e.ToString());
        }
    }
}
