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
            server.RegisterMiner(m);
            Console.WriteLine(server.GetCurrentMiners().Count);
            Console.WriteLine(server.GetCurrentUsers().Count);
            server.UnregisterMember(m);
            Console.WriteLine(server.GetCurrentMiners().Count);
            Console.WriteLine(server.GetCurrentUsers().Count);
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
