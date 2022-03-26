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
        public Miner() : base()
        {
            TransactionsQueue = new List<GenericTransaction>();
            FillWithRandomTransactions();
            CanMine = true;
            //while (true)
            //{
            //    MineNow();
            //}
        }
        //Auxiliar method for initializing all miners at once
        public override void Init()
        {

        }
        public void MineNow()
        {
            ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Mine(now);
        }
        //Receives time to try the generation in the same unit as block stamp (rn is ms), time must be equal or larger than the block stamp.
        protected void Mine(ulong time)
        {
            Console.WriteLine(CanMine);
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
                uint easing = (uint)(difference * 1); //The amount of "easing" per ms. Usually is one, meaning that each ms it is 1 iteration easier to get lucky
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
                        CanMine = false;
                        Console.WriteLine("Won the lottery!");
                        Console.WriteLine("Lucky value: " + luckyValue.ToString());
                        Console.WriteLine("Easing: " + easing.ToString());
                        Console.WriteLine("Lucky draw: " + luckyDraw.ToString()+"\n");
                        WonLottery(luckyDraws,i,time,Blockchain);
                        break;
                    }
                }
            }
        }
        protected void WonLottery(List<uint> luckyDraws, int winningDraw, ulong time, Blockchain blockchainToUse)
        {
            Block block = new Block(TransactionsQueue, Id, 0, time, blockchainToUse.LastBlock().GetHash());
            blockchainToUse.Add(block);
            Console.WriteLine("New blockchain length is: "+blockchainToUse.Count());
            blockchainToUse.PrettyPrint();
            AddBlockchainCandidate(blockchainToUse);
            //TO DO: Communicate to network
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
            //Right now just get largest chain
            Blockchain best = null;
            foreach (Blockchain chain in ChainCandidates)
            {
                if ((best == null) || (chain.Count() > best.Count()))
                {
                    best = chain;
                }
            }
            //TODO:
            /*
            //Remove transactions from the queue that were added
            foreach (Block bl in Blockchain.Chain)
            {
                foreach(GenericTransaction tr in TransactionsQueue.ToList())
                {
                    if (bl.Transactions.Contains(tr))
                    {
                        TransactionsQueue.Remove(tr);
                    }
                }
            }
            Console.WriteLine("New blockchain: ");
            Blockchain.PrettyPrint();
            FillWithRandomTransactions();
            canMine = true;
            */
            TransactionsQueue.Clear();
            FillWithRandomTransactions();
            CanMine = true;
        }
        private void FillWithRandomTransactions()
        {
            Random rd = new Random();
            int aux = rd.Next(1, 5);
            for (int i = 0; i < aux; i++)
            {
                GenericTransaction tr = new GenericTransaction();
                TransactionsQueue.Add(tr);
            }
        }
    }
}
