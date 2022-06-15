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
            PrevHash = GetGenesis().GetHash().Copy();
            DayStamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            Data = new List<GenericNaughtyEntry>();
        }
        public NaughtyListBlock(ByteArray prevHash, List<GenericNaughtyEntry> data)
        {
            DayStamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            PrevHash = prevHash.Copy();
            Data = new List<GenericNaughtyEntry>(data);
        }
        //Only use to create genesis block
        private NaughtyListBlock(ByteArray prevHash, List<GenericNaughtyEntry> data, ulong stamp)
        {
            DayStamp = stamp;
            PrevHash = prevHash.Copy();
            Data = new List<GenericNaughtyEntry>(data);
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
        public override string ToString()
        {
            string ret = "";
            foreach (GenericNaughtyEntry entry in Data)
            {
                ret += entry.ToString() + "\n";
            }
            return ret;
        }
    }
}
