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
            Testing.TestReadSpeed();
        }
        static void AllMinersMine(List<Miner> miners)
        {
            foreach (Miner miner in miners)
            {
                miner.MineNow();
            }
        }
        static void FreeRangeMining()
        {
            MainServer server = new MainServer();
            Miner m1 = new Miner(true); server.RegisterMiner(m1);
            Miner m2 = new Miner(); server.RegisterMiner(m2);
            Miner m3 = new Miner(); server.RegisterMiner(m3);
            Miner m4 = new Miner(); server.RegisterMiner(m4);
            foreach (Miner miner in server.GetCurrentMiners())
            {
                miner.Init(server);
                Console.WriteLine("User: "+miner.Id);
            }
            Console.WriteLine("Start");
            while (true)
            {
                AllMinersMine(server.GetCurrentMiners());
            }
        }
    }
}
