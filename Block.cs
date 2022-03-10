using ProofOfCredit.Transactions;
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
        public long Stamp;
        public List<Transaction> Transactions;
        public byte[] PrevHash;
        public Byte[] MinerId;
        public uint TimeGen;
        public byte Pv;
        public Block()
        {
        }
        public Byte[] GetHash()
        {
            byte[] data = new byte[2];
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);
            }
                return new Byte[256];
        }
    }
}
