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
        public ByteArray Id { get; protected set; }
        protected ByteArray PasswordHash;
        protected List<Members.Member> ListaUsuarios;
        protected List<Members.Member> ListaMineros;
        protected Dictionary<ByteArray, uint> CreditosDeMiembros;
        protected List<Object> ListaIds; //TODO: Change to actual transaction id type
        protected Blockchain Blockchain;
        protected List<Blockchain> ChainCandidates;
        public uint Credit { get; protected set; }
        public Member()
        {
            Random rd = new Random();
            Id = new ByteArray(BitConverter.GetBytes(rd.Next(1,2000)));
            PasswordHash = new ByteArray(ASCIIEncoding.ASCII.GetBytes("password"));
            ListaMineros = new List<Member>();
            ListaUsuarios = new List<Member>();
            Blockchain = new Blockchain();
            ListaIds = new List<object>();
            ChainCandidates = new List<Blockchain>();
            Credit = 10;
            CreditosDeMiembros = new Dictionary<ByteArray, uint>();
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
    }
}
