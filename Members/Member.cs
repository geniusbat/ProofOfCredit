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
        protected List<Block> Blockchain;
        public uint Credit { get; protected set; }
        public Member()
        {
            Id = new ByteArray(BitConverter.GetBytes(2243211));
            PasswordHash = new ByteArray(ASCIIEncoding.ASCII.GetBytes("password"));
            ListaMineros = new List<Member>();
            ListaUsuarios = new List<Member>();
            Blockchain = new List<Block>();
            Credit = 10;
            Blockchain.Add(Block.GetGenesis());
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
    }
}
