using ProofOfCredit.Members;
using ProofOfCredit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfCredit
{
    class MainServer : Miner 
    {
        //Previous PVs and their corresponding dates (as unix milisecond time) (Current PV must be +1 to the latests PV in this list).
        Dictionary<int, List<ulong>> PvDates;
        public MainServer() : base()
        {
            CanMine = false;
        }
        public void RegisterUser(User user)
        {
            UsersList.Add(user);
        }
        public void RegisterMiner(Miner miner)
        {
            MinersList.Add(miner);
        }
        public void UnregisterMember(Member member)
        {
            if (MinersList.Contains(member))
            {
                MinersList.Remove((Miner)member);
            }
            else if (UsersList.Contains(member))
            {
                UsersList.Remove((User)member);
            }
        }
    }
}
