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
                if (!(bl.IsValid()))
                {
                    return false;
                }
            }
            //TODO: Check for prevhash
            return true;
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
                ret += bl.ToString() + "\n------------------------------------\n";
            }
            Console.WriteLine(ret);
        }

    }
}
