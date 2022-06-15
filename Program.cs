using ProofOfCredit.Members;
using ProofOfCredit.NaughtyList;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProofOfCredit
{
    class Program
    {
        static void Main(string[] args)
        {
            Testing.TestBlockValidity();
            Testing.TestTransactionValidity();
            Testing.TestBlockchainValidity();
            Testing.TestMining();
        }
    }
}
