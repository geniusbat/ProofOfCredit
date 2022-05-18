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
            uint max = 0;
            Random rd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                int a = rd.Next(0, 256);
                int b = rd.Next(0, 256);
                byte[] luckyValueHash = new byte[4];
                luckyValueHash[0] = BitConverter.GetBytes(a)[0];
                luckyValueHash[1] = (byte)(Math.Max(b - 192, 0));
                luckyValueHash[2] = 0;
                luckyValueHash[3] = 0;
                uint res = BitConverter.ToUInt32(luckyValueHash);
                if (max < res)
                {
                    max = res;
                } 
            }
            Console.WriteLine("Max was: " + max);
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
                Console.WriteLine("User: " + miner.Id);
            }
            Console.WriteLine("Start");
            while (true)
            {
                AllMinersMine(server.GetCurrentMiners());
            }
        }
    }
}
