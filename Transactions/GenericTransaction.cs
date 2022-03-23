using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Transactions
{
    class GenericTransaction
    {
        public uint Id { get; protected set; } //TODO: Change to a more appropiate data type
        public ByteArray From { get; protected set; }
        public ByteArray To { get; protected set; }
        public uint Quantity  { get; protected set; }
        public ByteArray Sign  { get; protected set; }
        public GenericTransaction()
        {
            //TODO: Fill randomly
            Id = 0;
            Sign = new ByteArray();
        }
        public GenericTransaction(uint id, ByteArray from, ByteArray to, uint quantity, ByteArray sign)
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
