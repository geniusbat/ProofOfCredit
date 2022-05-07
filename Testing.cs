using Newtonsoft.Json;
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
            //For each block get a random transacion and look for it from beginning to end of chain
            for (int i = 0; i < blockCount; i++)
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
                    timer.Reset();
                }
            }
            Double totalTime = TimeSpan.FromTicks(averageTime).TotalMilliseconds;
            Console.WriteLine("The average lookup time for a transaction is: " + totalTime + "ms");
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
