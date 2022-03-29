using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            Random rd = new Random();
            Id = (uint)rd.Next(0,1000);
            From = new ByteArray();
            To = new ByteArray();
            From.FillRandomly(16);
            To.FillRandomly(16);
            Quantity = (uint)rd.Next(1, 10);
            Sign = new ByteArray();
            Sign.FillRandomly(20);
        }
        public GenericTransaction(uint id, ByteArray from, ByteArray to, uint quantity, ByteArray sign)
        {
            Id = id; From = from; To = to; Quantity = quantity; Sign = sign;
        }
        public ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = From;
                sum = sum.Sum(To);
                sum = sum.Sum(BitConverter.GetBytes(Id));
                sum = sum.Sum(Sign);
                sum = sum.Sum(BitConverter.GetBytes(Quantity));
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
        public bool IsValid ()
        {
            //TODO
            return true;
        }
        public override String ToString()
        {
            String ret = Id.ToString() +" From:"+ From.ToString() +" To:"+ To.ToString() +" Quantity:"+ Quantity.ToString() +" Sign:"+ Sign.ToString();
            return ret;
        }
        public bool Equals(GenericTransaction tr)
        {
            if ((Id==tr.Id)&(From.Equals(tr.From)&(To.Equals(tr.To))&(Quantity==tr.Quantity)&(Sign.Equals(tr.Sign))))
            {
                return true;
            }
            return false;
        }
        public GenericTransaction Copy()
        {
            return new GenericTransaction(Id,From.Copy(),To.Copy(), Quantity,Sign.Copy());
        }
    }
}
