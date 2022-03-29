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
            Console.WriteLine("After:\n"+bc);
            /*
            while(true)
            {
                m.MineNow();
            }*/
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
