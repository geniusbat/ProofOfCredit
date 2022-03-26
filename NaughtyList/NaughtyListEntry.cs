using ProofOfCredit.Transactions;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.NaughtyList
{
    public enum EntryType
    {
        BlockViolation,
        TransactionViolation,
        NetworkSaturation,
        Other
    }
    abstract class GenericNaughtyEntry
    {
        public ByteArray BlockHash { get; protected set; }
        public EntryType Type { get; protected set; }
        public ByteArray ResposibleId { get; protected set; }
        public int CreditLoss { get; protected set; }
        public virtual ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = BlockHash;
                sum = sum.Sum(ResposibleId);
                sum = sum.Sum(BitConverter.GetBytes(CreditLoss));
                //TODO: Sum Type
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
        public new virtual string ToString()
        {
            String ret = "";
            ret += "PrevHash: "+BlockHash.ToString();
            ret += " Type: " + Type.ToString();
            ret += " Responsible: " + ResposibleId.ToString();
            return ret;
        }
    }
    class BlockViolationEntry : GenericNaughtyEntry
    {
        public BlockViolationEntry()
        {
            BlockHash = new ByteArray(BitConverter.GetBytes(2));
            Type = EntryType.BlockViolation;
            ResposibleId = new ByteArray(BitConverter.GetBytes(12));
            CreditLoss = 10;
        }
        public BlockViolationEntry(ByteArray blockHash, Block block, ByteArray id)
        {
            BlockHash = blockHash;
            Type = EntryType.BlockViolation;
            ResposibleId = id;
            CreditLoss = 10;
            //Make sure the given block is invalid
            if (!(block.IsValid()))
            {
                //TODO: Handle exception
                return;
            }
        }
        public override ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = BlockHash;
                sum = sum.Sum(ResposibleId);
                sum = sum.Sum(BitConverter.GetBytes(CreditLoss));
                //TODO: Sum Type
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
    }
    class TransactionViolationEntry : GenericNaughtyEntry
    {
        public TransactionViolationEntry()
        {
            BlockHash = new ByteArray(BitConverter.GetBytes(31));
            Type = EntryType.BlockViolation;
            ResposibleId = new ByteArray(BitConverter.GetBytes(672));
            CreditLoss = 10;
        }
        public uint TransactionId { get; protected set; }
        public int TransactionPosition { get; protected set; }
        public TransactionViolationEntry(ByteArray blockHash, uint trId, Block block ,ByteArray id)
        {
            BlockHash = blockHash;
            Type = EntryType.BlockViolation;
            ResposibleId = id;
            CreditLoss = 10;
            TransactionId = trId;
            //Get Transaction position
            TransactionPosition = -4;
            for (int i = 0; i < block.Transactions.Count; i++)
            {
                GenericTransaction tr = block.Transactions[i];
                if (tr.Id == trId)
                {
                    TransactionPosition = i;
                    break;
                }
            }
            //Make sure the transaction's position was found inside the list, thus it must be different to -4.
            if (TransactionPosition==-4)
            {
                //TODO: Handle exception
                return;
            }
            //Make sure the transaction is invalid
            if (!(block.Transactions[TransactionPosition].IsValid()))
            {
                //TODO: Handle exception
                return;
            }
        }
        public override ByteArray GetHash()
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                ByteArray sum = BlockHash;
                sum = sum.Sum(ResposibleId);
                sum = sum.Sum(BitConverter.GetBytes(CreditLoss));
                sum = sum.Sum(BitConverter.GetBytes(TransactionId));
                sum = sum.Sum(BitConverter.GetBytes(TransactionPosition));
                //TODO: Sum Type
                hash = sha.ComputeHash(sum.Bytes);
            }
            return new ByteArray(hash);
        }
    }
}
