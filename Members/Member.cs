using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit.Members
{
    class Member
    {
        public byte[] Id;
        public byte[] PasswordHash;
        private List<Members.Member> ListaUsuarios;
        private List<Members.Member> ListaMineros;
        private List<Object> ListaIds; //TODO: Change to actual transaction id type
        public List<Block> Blockchain;
    }
}
