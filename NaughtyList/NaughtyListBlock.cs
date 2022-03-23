using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.NaughtyList
{
    class NaughtyListBlock
    {
        public ulong DayStamp;
        public List<GenericNaughtyEntry> Data;
        public ByteArray PrevHash;
        public NaughtyListBlock()
        {
            PrevHash = GetGenesis().GetHash();
            DayStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //TODO: Modulate to unix time from the beginning of the day
            Data = new List<GenericNaughtyEntry>();
        }
        public NaughtyListBlock(ByteArray prevHash, List<GenericNaughtyEntry> data)
        {
            DayStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //TODO: Modulate to unix time from the beginning of the day
            PrevHash = prevHash;
            Data = data;
        }
        //Only use to create genesis block
        private NaughtyListBlock(ByteArray prevHash, List<GenericNaughtyEntry> data, ulong stamp)
        {
            DayStamp = stamp;
            PrevHash = prevHash;
            Data = data;
        }
        public ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = PrevHash;
                sum = sum.Sum(BitConverter.GetBytes(DayStamp));
                //Get sum of hashes from transactions
                foreach (GenericNaughtyEntry element in Data)
                {
                    sum = sum.Sum(element.GetHash());
                }
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
        static public NaughtyListBlock GetGenesis()
        {
            DateTime originalDate = new DateTime(2021, 8, 7);
            ulong stamp = (ulong)((DateTimeOffset)originalDate).ToUnixTimeSeconds();
            NaughtyListBlock bl = new NaughtyListBlock(new ByteArray(BitConverter.GetBytes(0)), new List<GenericNaughtyEntry>(), stamp);
            return bl;
        }
    }
}
