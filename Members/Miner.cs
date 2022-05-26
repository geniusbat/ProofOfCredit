using Newtonsoft.Json;
using ProofOfCredit.Transactions;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Members
{
    class Miner : Member
    {
        private List<GenericTransaction> TransactionsQueue; 
        protected bool CanMine;
        private bool testMiner;
        public Miner(bool test) : base()
        {
            TransactionsQueue = new List<GenericTransaction>();
            FillWithRandomTransactions();
            CanMine = true;
            testMiner = test;
        }
        public Miner() : base()
        {
            TransactionsQueue = new List<GenericTransaction>();
            FillWithRandomTransactions();
            CanMine = true;
            testMiner = false;
        }
        //Auxiliar method for initializing all miners at once
        public override void Init(MainServer server)
        {
            base.Init(server);
        }
        public void MineNow()
        {
            ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Mine(now);
        }
        //Receives time to try the generation in the same unit as block stamp (rn is ms), time must be equal or larger than the block stamp.
        protected void Mine(ulong time)
        {
            if (!CanMine)
            {
                return;
            }
            Block lastBlock = Blockchain.LastBlock();
            //Console.Write("Mining on block hash: "); lastBlock.GetHash().PrettyPrint();
            ulong blockStamp = lastBlock.Stamp;
            if (time >= blockStamp)
            {
                //Remember that stamp are ms not s
                ulong difference = time - blockStamp;
                uint easing = (uint)(difference / 100);
                //Get lucky draws
                uint luckyDrawsAmount = GetLuckyDraws(Credit);
                List<uint> luckyDraws = new List<uint>();
                //Generate each lucky draw
                for (int i = 0; i < luckyDrawsAmount; i++)
                {
                    string data = ASCIIEncoding.ASCII.GetString(Id.Bytes)+i.ToString()+ASCIIEncoding.ASCII.GetString(lastBlock.GetHash().Bytes);
                    using (SHA256 sha = SHA256.Create())
                    {
                        byte[] hash = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(data));
                        byte[] aux = new byte[4];
                        aux[0] = hash[0];
                        aux[1] = hash[1];
                        aux[2] = 0;
                        aux[3] = 0;
                        luckyDraws.Add(BitConverter.ToUInt32(aux,0));
                    }
                }
                //Get lucky value from block
                uint luckyValue = lastBlock.GetLuckyValue();
                //Check if luckyValue is equal or bigger to any lucky draw
                for (int i = 0; i < luckyDraws.Count(); i++)
                {
                    uint luckyDraw = luckyDraws[0];
                    if (luckyValue+easing>=luckyDraw)
                    {
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine(Id+" Won the lottery!");
                        Console.WriteLine("Lucky value: " + luckyValue.ToString());
                        Console.WriteLine("Easing: " + easing.ToString());
                        Console.WriteLine("Lucky draw: " + luckyDraw.ToString() + "\n");
                        WonLottery(luckyDraws,i,time,Blockchain);
                        break;
                    }
                }
            }
        }
        protected void WonLottery(List<uint> luckyDraws, int winningDraw, ulong time, Blockchain blockchainToUse)
        {
            CanMine = false;
            Block block = new Block(TransactionsQueue, Id, 0, time, blockchainToUse.LastBlock().GetHash());
            System.IO.File.WriteAllText(@"E:\Proyectos\Programación\ProofOfCredit\ProofOfCredit\Data\serializedBlock.txt",block.Serialize());
            if (testMiner)
            {
                Console.WriteLine("New block generated will be: \n" + block);
            }
            AddBlockchainCandidate(block);
            //Communicate to network
            foreach(Miner miner in MinersList)
            {
                miner.AddBlockchainCandidate(block);
            }
            if (Server!=null)
            {
                Server.AddBlockchainCandidate(block);
            }
        }
        public void AddTransactionToQueue(GenericTransaction tr)
        {
            if (tr.IsValid())
            {
                if (!(TransactionsQueue.Contains(tr)))
                {
                    TransactionsQueue.Add(tr);
                }
            }
        }
        public override void CheckToUpdateOficialChain()
        {
            CanMine = false;
            int[] chainCredits = new int[ChainCandidates.Count()];
            for (int i = 0; i < ChainCandidates.Count(); i++)
            {
                Blockchain chain = ChainCandidates[i];
                int sum = 0;
                foreach (Block bl in chain.Chain)
                {
                    if (CreditsPerMember.ContainsKey(bl.MinerId))
                    {
                        sum += CreditsPerMember[bl.MinerId];
                    }
                }
                chainCredits[i] = sum;
            }
            //make sure there were candidates being considered
            if (chainCredits.Length > 0)
            {
                int posOfBest = 0;
                for (int i = 0; i < chainCredits.Length; i++)
                {
                    if (chainCredits[posOfBest] > chainCredits[i])
                    {
                        posOfBest = i;
                    }
                }
                //The chain with the highest credit was found, make it the official one
                Blockchain = ChainCandidates[posOfBest];
                ChainCandidates.Clear();
            }
            //Remove transactions from the queue that were added
            List<GenericTransaction> intersection = new List<GenericTransaction>();
            foreach (Block bl in Blockchain.Chain)
            {
                intersection.AddRange(TransactionsQueue.Intersect(bl.Transactions));
            }
            foreach (GenericTransaction tr in intersection)
            {
                TransactionsQueue.Remove(tr);
            }
            if (testMiner)
            {
                //Console.WriteLine("===========================================================================================\nNew blockchain:\n" + Blockchain);
            }
            FillWithRandomTransactions();
            CanMine = true;
        }
        private void FillWithRandomTransactions()
        {
            Random rd = new Random();
            int aux = rd.Next(2, 6);
            if (!(aux % 2 == 0))
            {
                aux += 1;
            }
            for (int i = 0; i < aux; i++)
            {
                GenericTransaction tr = new GenericTransaction();
                TransactionsQueue.Add(tr);
            }
        }
    }
}
