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
        private List<Transaction> TransactionsQueue;
        public Miner() : base()
        {
            TransactionsQueue = new List<Transaction>();
            while (true)
            {
                MineNow();
            }
        }
        private void MineNow()
        {
            ulong now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Mine(now);
        }
        //Receives time to try the generation in the same unit as block stamp (rn is ms), time must be equal or larger than the block stamp.
        private void Mine(ulong time)
        {
            Block lastBlock = Blockchain[Blockchain.Count()-1];
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
                /*
                Console.Write("Lucky draws:");
                foreach (uint luckyDraw in luckyDraws)
                {
                    Console.Write(luckyDraw.ToString()+" ");
                }
                Console.WriteLine(); */
                //Get lucky value from block
                uint luckyValue = lastBlock.GetLuckyValue();
                //Console.WriteLine("Lucky value and easing: "+(luckyValue + easing).ToString());
                //Check if luckyValue is equal or bigger to any lucky draw
                for (int i = 0; i < luckyDraws.Count(); i++)
                {
                    uint luckyDraw = luckyDraws[0];
                    if (luckyValue+easing>=luckyDraw)
                    {
                        Console.WriteLine("Won the lottery!");
                        WonLottery(luckyDraws,i,time,Blockchain);
                        break;
                    }
                }
            }
        }
        private void WonLottery(List<uint> luckyDraws, int winningDraw, ulong time, List<Block> blockchainToUse)
        {
            Block block = new Block(TransactionsQueue, Id, 0, time, blockchainToUse[blockchainToUse.Count - 1].GetHash());
            TransactionsQueue.Clear();
            blockchainToUse.Add(block);
            //TO DO: Communicate to network
        }
        public void AddTransactionToQueue(Transaction tr)
        {
            if (tr.IsValid())
            {
                if (!(TransactionsQueue.Contains(tr)))
                {
                    TransactionsQueue.Add(tr);
                }
            }
        }
    }
}
