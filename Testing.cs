using Newtonsoft.Json;
using ProofOfCredit.Members;
using ProofOfCredit.Transactions;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit
{
    class Testing
    {

        public static void TestBlockValidity()
        {
            Random rd = new Random();
            Miner miner = new Miner();
            ulong stamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            List<GenericTransaction> trs = new List<GenericTransaction>();
            for (int i = 0; i < 4; i++)
            {
                GenericTransaction tr = new GenericTransaction();
                trs.Add(tr);
            }
            //Random block
            Block b = new Block(trs,miner.Id,BitConverter.GetBytes(0)[0],stamp,Block.GetGenesis().GetHash());
            if (!(b.IsValid() == true))
            {
                Console.WriteLine("Block validity false");
            }
            else
            {
                Console.WriteLine("Block validity correct");
            }
        }
        public static void TestTransactionValidity()
        {
            Random rd = new Random();
            uint id = (uint)rd.Next(0, 1000);
            ByteArray from = new ByteArray();
            ByteArray to = new ByteArray();
            from.FillRandomly(16);
            to.FillRandomly(16);
            uint quantity = (uint)rd.Next(1, 10);
            ByteArray sign = new ByteArray();
            sign.FillRandomly(20);
            GenericTransaction tr = new GenericTransaction(id, from, to, quantity, sign);
            //Console.WriteLine("Transaction hash: "+tr.GetHash());
            if ((from.Equals(tr.From)) & (to.Equals(tr.To)) & (id == (tr.Id)) & (quantity == (tr.Quantity)) & (sign.Equals(tr.Sign)))
            {
                Console.WriteLine("Transaction validity correct");
            }
            else
            {
                Console.WriteLine("Transaction validity false");
            }
        }
        public static void TestBlockchainValidity()
        {
            Random rd = new Random();
            //Generate a few random blocks
            Miner miner = new Miner();
            ulong stamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            List<GenericTransaction> trs = new List<GenericTransaction>();
            List<GenericTransaction> trs2 = new List<GenericTransaction>();
            for (int i = 0; i < 4; i++)
            {
                GenericTransaction tr = new GenericTransaction();
                GenericTransaction tr2 = new GenericTransaction();
                trs.Add(tr);
                trs2.Add(tr2);
            }
            Block b1 = new Block(trs, miner.Id, BitConverter.GetBytes(0)[0], stamp, Block.GetGenesis().GetHash());
            Block b2 = new Block(trs2, miner.Id, BitConverter.GetBytes(0)[0], stamp, b1.GetHash());
            Blockchain bl = new Blockchain();
            bl.Add(b1);
            bl.Add(b2);
            if (!(bl.IsValid()))
            {
                Console.WriteLine("Blockchain validity false");
            }
            else
            {
                Console.WriteLine("Blockchain validity correct");
            }
        }
        public static void TestMining()
        {
            //Instance network
            List<Miner> miners = new List<Miner>();
            MainServer server = new MainServer();
            Miner m1 = new Miner(true); server.RegisterMiner(m1);
            Miner m2 = new Miner(); server.RegisterMiner(m2);
            Miner m3 = new Miner(); server.RegisterMiner(m3);
            Miner m4 = new Miner(); server.RegisterMiner(m4);
            foreach (Miner miner in server.GetCurrentMiners())
            {
                miner.Init(server);
                Console.WriteLine("User: " + miner.Id);
                miners.Add(miner);
            }
            Console.WriteLine("Start");
            while (true)
            {
                foreach (Miner miner in miners)
                {
                    miner.MineNow();
                }
            }
        }

        public static void RandomTestReadSpeed()
        {
            //Get a random transaction and try to look for it in the blockchain block by block (obviously it is a brute way but it is the default method).
            Root blockchain = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(@"E:\Proyectos\Programación\ProofOfCredit\ProofOfCredit\Data\blockchain.txt"));
            Console.WriteLine("There are "+ blockchain.Chain.Count()+" blocks");
            Random rd = new Random();
            Stopwatch timer = new Stopwatch();
            int epochs = 10000;
            long averageTime = 0;
            for (int i = 0; i < epochs; i++)
            {
                int randomBlockPos = rd.Next(0, (int)blockchain.Chain.Count());
                List<Transaction> trs = blockchain.Chain[randomBlockPos].Transactions;
                Transaction tr = null;
                if (trs.Count() >0)
                {
                    tr = trs[rd.Next(0, trs.Count())];
                }
                //Search for transaction from the latests block
                if (tr!=null)
                {
                    timer.Start();
                    //Search
                    List<Chain> reversedChain = new List<Chain>(blockchain.Chain);
                    reversedChain.Reverse();
                    foreach (Chain bl in reversedChain)
                    {
                        if (bl.Transactions.Contains(tr))
                        {
                            break;
                        }
                    }
                    timer.Stop();
                    if (averageTime==0.0)
                    {
                        averageTime += timer.ElapsedTicks;
                    }
                    else
                    {
                        averageTime = (averageTime + timer.ElapsedTicks) / 2; 
                    }
                }
            }
            Double totalTime = TimeSpan.FromTicks(averageTime).TotalSeconds;
            Console.WriteLine("The average lookup time for a transaction is: "+ totalTime);
            string content = "For "+epochs+" transactions"+" wait time was: "+ totalTime;
            System.IO.File.WriteAllText(@"E:\Proyectos\Programación\ProofOfCredit\ProofOfCredit\Data\averageLookUpTime.txt",content);
        }
        public static void TestReadSpeed()
        {
            Root blockchain = JsonConvert.DeserializeObject<Root>(System.IO.File.ReadAllText(@"E:\Proyectos\Programación\ProofOfCredit\ProofOfCredit\Data\blockchain.txt"));
            int blockCount = blockchain.Chain.Count();
            Console.WriteLine("There are " + blockCount + " blocks");
            Random rd = new Random();
            Stopwatch timer = new Stopwatch();
            long averageTime = 0;
            long maxTicks = 0;
            long minTicks = -1;
            //For each block get a random transacion and look for it from beginning to end of chain. Skip genesis block
            for (int i = 1; i < blockCount; i++)
            {
                List<Transaction> trs = blockchain.Chain[i].Transactions;
                Transaction tr = null;
                //Get a random transction if they are available in the block
                if (trs.Count() > 0)
                {
                    tr = trs[rd.Next(0, trs.Count())];
                }
                //Search for transaction from the latests block to the oldest
                if (tr != null)
                {
                    List<Chain> reversedChain = new List<Chain>(blockchain.Chain);
                    reversedChain.Reverse();
                    timer.Start();
                    //Search
                    foreach (Chain bl in reversedChain)
                    {
                        if (bl.Transactions.Contains(tr))
                        {
                            break;
                        }
                    }
                    timer.Stop();
                    if (averageTime == 0.0)
                    {
                        averageTime += timer.ElapsedTicks;
                    }
                    else
                    {
                        averageTime = (averageTime + timer.ElapsedTicks) / 2;
                    }
                    if (timer.ElapsedTicks > maxTicks)
                    {
                        maxTicks = timer.ElapsedTicks;
                    }
                    else if ((timer.ElapsedTicks < minTicks) || (minTicks==-1))
                    {
                        minTicks = timer.ElapsedTicks;
                    }
                    timer.Reset();
                }
            }
            Double totalTime = TimeSpan.FromTicks(averageTime).TotalMilliseconds;
            Console.WriteLine("The average lookup time for a transaction is: " + totalTime + "ms");
            Console.WriteLine("Largest wait time in ms was: "+ TimeSpan.FromTicks(maxTicks).TotalMilliseconds);
            Console.WriteLine("Smallest wait time in ms was: " + TimeSpan.FromTicks(minTicks).TotalMilliseconds);
            string content = "The average lookup time for a transaction is: " + totalTime + "ms";
            System.IO.File.WriteAllText(@"E:\Proyectos\Programación\ProofOfCredit\ProofOfCredit\Data\averageLookUpTime.txt", content);
        }
        public class From
        {
            public string Bytes { get; set; }
            public int Length { get; set; }
        }

        public class To
        {
            public string Bytes { get; set; }
            public int Length { get; set; }
        }

        public class Sign
        {
            public string Bytes { get; set; }
            public int Length { get; set; }
        }

        public class Transaction
        {
            public int Id { get; set; }
            public From From { get; set; }
            public To To { get; set; }
            public int Quantity { get; set; }
            public Sign Sign { get; set; }
        }

        public class PrevHash
        {
            public string Bytes { get; set; }
            public int Length { get; set; }
        }

        public class MinerId
        {
            public string Bytes { get; set; }
            public int Length { get; set; }
        }

        public class Chain
        {
            public object Stamp { get; set; }
            public List<Transaction> Transactions { get; set; }
            public PrevHash PrevHash { get; set; }
            public MinerId MinerId { get; set; }
            public object TimeGen { get; set; }
            public int PV { get; set; }
        }

        public class Root
        {
            public List<Chain> Chain { get; set; }
        }
    }

}
