using ProofOfCredit.NaughtyList;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Members
{
    class Member
    {
        //Helper variable for local tests
        public MainServer server = null;
        public ByteArray Id { get; protected set; }
        //TODO: Add IP property
        protected ByteArray PasswordHash;
        protected List<User> UsersList;
        protected List<Miner> MinersList;
        protected Dictionary<ByteArray, int> CreditsPerMember;
        protected List<Object> ListaIds; //TODO: Change to actual transaction id type
        protected Blockchain Blockchain;
        protected List<Blockchain> ChainCandidates;
        protected List<NaughtyListBlock> NaughtyList;
        protected byte CurrentPV;
        public uint Credit { get; protected set; }
        public Member()
        {
            Random rd = new Random();
            Id = new ByteArray(BitConverter.GetBytes(rd.Next(1,2000)));
            PasswordHash = new ByteArray(ASCIIEncoding.ASCII.GetBytes("password"));
            MinersList = new List<Miner>();
            UsersList = new List<User>();
            Blockchain = new Blockchain();
            NaughtyList = new List<NaughtyListBlock>();
            ListaIds = new List<object>();
            ChainCandidates = new List<Blockchain>();
            Credit = 10;
            CreditsPerMember = new Dictionary<ByteArray, int>();
            CurrentPV = BitConverter.GetBytes(0)[0];
        }
        public virtual void Init()
        {
            //Get data
            if (server!=null)
            {
                MinersList = server.GetCurrentMiners();
                UsersList = server.GetCurrentUsers();
                CreditsPerMember = server.GetCreditsPerMember();
            }
            else
            {
                Console.WriteLine("Server was null");
            }
        }
        public List<User> GetCurrentUsers()
        {
            return UsersList;
        }
        public List<Miner> GetCurrentMiners()
        {
            return MinersList;
        }
        public Dictionary<ByteArray, int> GetCreditsPerMember()
        {
            return CreditsPerMember;
        }
        protected void CalculateMembersCredits()
        {
            //Add credit for each block generated
            Dictionary<ByteArray, int> creditScores = new Dictionary<ByteArray, int>();
            foreach (Block bl in Blockchain.Chain)
            {
                //Make sure block is valid
                if (!(bl.IsValid()))
                {
                    ViolationFound(bl);
                    return;
                }
                //Miner already in dict
                if (creditScores.ContainsKey(bl.MinerId))
                {
                    creditScores[bl.MinerId] += 1;
                }
                //First time we encounter the miner
                else
                {
                    creditScores[bl.MinerId] = 1;
                }
            }
            //Substract credit from violations
            foreach (NaughtyListBlock bl in NaughtyList)
            {
                foreach(GenericNaughtyEntry entry in bl.Data)
                {
                    ByteArray id = entry.ResposibleId;
                    if (creditScores.ContainsKey(id))
                    {
                        creditScores[id] -= entry.CreditLoss;
                    }
                    else
                    {
                        creditScores[id] =  -entry.CreditLoss;
                    }
                }
            }
            CreditsPerMember = creditScores;
        }
        //Deprecated, better use version with block argument.
        public void AddBlockchainCandidate(Blockchain candidate)
        {
            if (candidate.IsValid())
            {
                ChainCandidates.Add(candidate);
            }
            //Update oficial
            CheckToUpdateOficialChain();
        }
        public virtual void AddBlockchainCandidate(Block newBlock)
        {
            if (newBlock.IsValid())
            {
                ByteArray prevHash = newBlock.PrevHash;
                //Look for blockchain candidate whose lasts block is equal to the new's block previous hash
                foreach (Blockchain blockchain in ChainCandidates)
                {
                    if (blockchain.LastBlock().GetHash().Equals(prevHash))
                    {
                        //Make sure new block is younger than lasts block
                        if (blockchain.LastBlock().Stamp<=newBlock.TimeGen)
                        {
                            blockchain.Add(newBlock);
                            //Make sure resulting blockchain is valid, if not revert to previous state
                            if (blockchain.IsValid())
                            {
                                CheckToUpdateOficialChain();
                                //Increase miner's credit
                                ByteArray id = newBlock.MinerId;
                                if (CreditsPerMember.ContainsKey(id))
                                {
                                    CreditsPerMember[id] += 1;
                                }
                                else
                                {
                                    CreditsPerMember[id] = 1;
                                }
                                return;
                            }
                            else
                            {
                                blockchain.Chain.Remove(newBlock);
                            }
                        }
                    }
                }
                //If it exited the foreach loop it means it wasn't found inside the candidates
                //Check if newBlock is an extension of the current chain
                if (Blockchain.LastBlock().GetHash().Equals(prevHash))
                {
                    if (Blockchain.LastBlock().Stamp <= newBlock.TimeGen)
                    {
                        Blockchain.Add(newBlock);
                        //Make sure resulting blockchain is valid, if not revert to previous state
                        if (Blockchain.IsValid())
                        {
                            ChainCandidates.Add(Blockchain);
                            CheckToUpdateOficialChain();
                            //Increase miner's credit
                            ByteArray id = newBlock.MinerId;
                            if (CreditsPerMember.ContainsKey(id))
                            {
                                CreditsPerMember[id] += 1;
                            }
                            else
                            {
                                CreditsPerMember[id] = 1;
                            }
                            return;
                        }
                        else
                        {
                            Blockchain.Chain.Remove(newBlock);
                        }
                    }
                }
            }
            //If here, block was invalid
        }
        //TODO
        public virtual void CheckToUpdateOficialChain()
        {
            //Right now just get largest chain
            Blockchain best = null;
            foreach (Blockchain chain in ChainCandidates)
            {
                if ((best==null) || (chain.Count()>best.Count()))
                {
                    best = chain;
                }
            }
        }
        //TODO
        protected void ViolationFound(Block invalidBlock)
        {
            return;
        }
        //Static calls
        public uint GetLuckyDraws(uint credits)
        {
            return GetLuckyDraws(credits, CurrentPV);
        }
        public static uint GetLuckyDraws(uint credits, byte pv)
        {
            int pvNum = (int)pv;
            switch (pvNum)
            {
                case 0:
                    if (credits <= 5)
                    {
                        return 0;
                    }
                    else if (credits <= 10)
                    {
                        return 1;
                    }
                    else if (credits <= 20)
                    {
                        return 2;
                    }
                    else if (credits <= 50)
                    {
                        return 3;
                    }
                    else
                    {
                        return 5;
                    }
                default:
                    if (credits <= 5)
                    {
                        return 0;
                    }
                    else if (credits <= 10)
                    {
                        return 1;
                    }
                    else if (credits <= 20)
                    {
                        return 2;
                    }
                    else if (credits <= 50)
                    {
                        return 3;
                    }
                    else
                    {
                        return 5;
                    }
            }
        }
        public bool CanMemberBranchOff(uint credits)
        {
            return CanMemberBranchOff(credits, CurrentPV);
        }
        public static bool CanMemberBranchOff(uint credits, byte pv)
        {
            int pvNum = (int)pv;
            switch (pvNum)
            {
                case 0:
                    if (credits > 20)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    if (credits > 20)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }
    }
}
