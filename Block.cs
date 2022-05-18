﻿using ProofOfCredit.Transactions;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit
{
    class Block
    {
        public ulong Stamp;
        public List<GenericTransaction> Transactions;
        public ByteArray PrevHash;
        public ByteArray MinerId;
        public ulong TimeGen;
        public byte PV;
        public Block(List<GenericTransaction> listOfTransactions, ByteArray miner, byte currentPv, ulong timeOfCreation, ByteArray prevHash)
        {
            Stamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); //Stamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Transactions = new List<GenericTransaction>(listOfTransactions);
            MinerId = miner.Copy();
            TimeGen = timeOfCreation;
            PV = currentPv;
            TimeGen = timeOfCreation;
            PrevHash = prevHash.Copy();
        }
        //Never really use this constructor unless for generating the genesis block (and providing auxiliar functionality)
        public Block(List<GenericTransaction> listOfTransactions, ByteArray miner, byte currentPv, ulong timeOfCreation, ByteArray prevHash, ulong stampGen)
        {
            Stamp = (ulong)stampGen;
            Transactions = new List<GenericTransaction>(listOfTransactions);
            MinerId = miner.Copy();
            TimeGen = timeOfCreation;
            PV = currentPv;
            TimeGen = timeOfCreation;
            PrevHash = prevHash.Copy();
        }
        public ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = PrevHash.Sum(MinerId);
                byte[] aux = new byte[1];
                aux[0] = PV;
                sum = sum.Sum(aux);
                sum = sum.Sum(BitConverter.GetBytes(TimeGen));
                sum = sum.Sum(BitConverter.GetBytes(Stamp));
                //Get sum of hashes from transactions
                foreach (GenericTransaction tr in Transactions)
                {
                    sum = sum.Sum(tr.GetHash());
                }
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
        //Better use the blockchain class "IsBlockValid"
        public bool IsValid()
        {
            bool ret = true;
            //Transactions must'nt be empty unless genesis block
            if ((Transactions.Count==0)&!(MinerId.Equals(Block.GetGenesis().MinerId)))
            {
                ret = false;
            }
            //Time of generation must be earlier or equal than time of block creation
            else if (!(TimeGen.CompareTo(Stamp)<=0))
            {
                ret = false;
            }
            foreach (GenericTransaction tr in Transactions)
            {
                if (!(tr.IsValid())) {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        public uint GetLuckyValue()
        {
            byte[] luckyValueHash = new byte[4];
            luckyValueHash[0] = GetHash().Bytes[0];
            luckyValueHash[1] = (byte)(Math.Max(GetHash().Bytes[1] - 192, 0));
            luckyValueHash[2] = 0;
            luckyValueHash[3] = 0;
            return BitConverter.ToUInt32(luckyValueHash);
        }
        static public Block GetGenesis()
        {
            DateTime originalDate = new DateTime(2021,8,7);
            ulong originalStamp = (ulong)((DateTimeOffset)originalDate).ToUnixTimeMilliseconds();
            List<GenericTransaction> noneTr = new List<GenericTransaction>();
            byte[] originalId = new byte[32];
            for (int i = 0; i < originalId.Length; i++)
            {
                originalId[i]= 0;
            }
            byte[] prevHash = new byte[32];
            return new Block(noneTr, new ByteArray(originalId), 0, originalStamp, new ByteArray(prevHash), originalStamp);
        }
        public override string ToString()
        {
            String ret = "";
            ret += "Hash: " + GetHash().ToString();
            ret += "\nPrevious: " + PrevHash.ToString();
            ret += "\nMiner: " + MinerId.ToString();
            ret += "\nStamp: " + Stamp.ToString();
            ret += "\nTransactions: \n";
            foreach(GenericTransaction tr in Transactions)
            {
                ret += tr.ToString()+"\n";
            }
            return ret;
        }
        public bool Equals(Block bl)
        {
            return GetHash() == bl.GetHash();
        }
    }
}
