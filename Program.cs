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
            //Miner m = new Miner();
            MainServer server = new MainServer();
            Miner m = new Miner();
            Blockchain c = new Blockchain();
            List<Transactions.GenericTransaction> l = new List<Transactions.GenericTransaction>();
            l.Add(new Transactions.GenericTransaction());
            Block b = new Block(l, new ByteArray(BitConverter.GetBytes(421)), BitConverter.GetBytes(0)[0], (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), c.Chain[0].GetHash());
            c.Add(b);
            Console.WriteLine(c);
            Console.WriteLine(c.IsValid());
        }
        static void AllMinersMine(List<Miner> miners)
        {
            foreach (Miner miner in miners)
            {
                miner.MineNow();
            }
        }
    }
}
