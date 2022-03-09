using ProofOfCredit.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit
{
    class Block
    {
        public long stamp;
        public List<Transaction> transactions;
        public byte[] prevHash;
        public Members.Miner miner;
        public uint timeGen;
        public byte pv;
        public Block()
        {
        }
    }
}
