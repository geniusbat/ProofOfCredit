using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Transactions
{
    class Transaction
    {
        public uint Id; //TODO: Change to a more appropiate data type
        public ByteArray From;
        public ByteArray To;
        public uint Quantity;
        public ByteArray Sign;
        public Transaction()
        {
            //TODO: Fill randomly
            Id = 0;
            Sign = new ByteArray();
        }
        public Transaction(uint id, ByteArray from, ByteArray to, uint quantity, ByteArray sign)
        {
            Id = id; From = from; To = to; Quantity = quantity; Sign = sign;
        }
        public ByteArray GetHash()
        {
            //TODO
            return new ByteArray(BitConverter.GetBytes(Id + Quantity));
        }
        public bool IsValid ()
        {
            //TODO
            return true;
        }
    }
}
