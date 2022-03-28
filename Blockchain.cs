using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit
{
    class Blockchain
    {
        public List<Block> Chain { get; private set; }
        public Blockchain()
        {
            Chain = new List<Block>();
            Chain.Add(Block.GetGenesis());
        }
        public bool IsValid()
        {
            foreach (Block bl in Chain)
            {
                if (!(IsBlockValid(bl)))
                {
                    Console.WriteLine("Error in: "+bl);
                    return false;
                }
            }
            //TODO: Check for prevhash
            return true;
        }
        public bool IsBlockValid(int blockPos)
        {
            //Basic block-contained validity
            bool ret = Chain[blockPos].IsValid();
            //(if not genesis (which must be the first block))
            if (blockPos == 0)
            {
                //The block is the genesis block (MinerId is all to 0).
                if (!Chain[blockPos].MinerId.Equals(Block.GetGenesis().MinerId))
                {
                    ret = false;
                }
            }
            //Make sure the previous block exists (if not genesis (which must be the first block))
            else
            {
                //Just check previous pos
                if ((blockPos != 0) | (Chain[blockPos - 1].GetHash() == Chain[blockPos].PrevHash))
                {
                    ret = true;
                }
                //Look for prevHash inside chain
                else
                {
                    //Not found 
                    ret = false;
                    ByteArray prevHash = Chain[blockPos].PrevHash;
                    //Set to true if found
                    foreach (Block bl in Chain)
                    {
                        if (bl.GetHash() == prevHash)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            return ret;
        }
        //Better use this instead of "IsValid" when wanting to get invalid blocks as it will return a list of all violating blocks.
        public List<Block> GetViolatingBlocks()
        {
            List<Block> ret = new List<Block>();
            foreach (Block bl in Chain)
            {
                if (!(IsBlockValid(bl)))
                {
                    ret.Add(bl);
                }
            }
            return ret;
        }
        public bool IsBlockValid(Block bl)
        {
            //Basic block-contained validity
            bool ret = bl.IsValid();
            //The block is the genesis block (MinerId is all to 0).
            if (bl.MinerId.Equals(Block.GetGenesis().MinerId))
            {
                
            }
            //Make sure the previous block exists (if not genesis (which must be the first block))
            else
            {
                int blockPos = Chain.IndexOf(bl);
                //This will ret=false if blockPos = 0
                //Just check previous pos
                if ((blockPos != 0) | (Chain[blockPos - 1].GetHash() == Chain[blockPos].PrevHash))
                {
                    ret = true;
                }
                //Look for prevHash inside chain 
                else
                {
                    //Not found 
                    ret = false;
                    ByteArray prevHash = bl.PrevHash;
                    //Set to true if found
                    foreach (Block element in Chain)
                    {
                        if (element.GetHash() == prevHash)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            return ret;
        }
        public uint Count()
        {
            return (uint)Chain.Count();
        }
        public void Add(Block bl)
        {
            //TODO bl points to previous block
            if (bl.IsValid()) {
                Chain.Add(bl);
            }
        }
        public Block LastBlock()
        {
            return Chain[Chain.Count() - 1];
        }
        public override string ToString()
        {
            String ret = "";
            foreach(Block bl in Chain)
            {
                ret += bl.ToString()+"\n";
            }
            return ret;
        }
        public void PrettyPrint()
        {
            String ret = "";
            foreach (Block bl in Chain)
            {
                ret += "Block: " + bl.ToString() + "\n------------------------------------\n";
            }
            Console.WriteLine(ret);
        }
    }
}
