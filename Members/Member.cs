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
        public static uint GetLuckyDraws(uint credits)
        {
            if (credits <= 5 )
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
        public void AddBlockchainCandidate(Blockchain candidate)
        {
            if (candidate.IsValid())
            {
                ChainCandidates.Add(candidate);
            }
            //Update oficial
            CheckToUpdateOficialChain();
        }
        //TODO
        public void AddBlockchainCandidate(Block newBlock)
        {

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
    }
}
