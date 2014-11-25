using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines
{
    public class PartyEngine
    {
        public World.Network.WorldClient Leader = null;
        public List<World.Network.WorldClient> Members = null;

        public PartyEngine(World.Network.WorldClient leader, World.Network.WorldClient invitedMember)
        {
            this.Leader = leader;
            Members = new List<World.Network.WorldClient>() { leader, invitedMember };
            leader.Character.Party = this;
            invitedMember.Character.Party = this;
            RefreshParty();
        }

        public void RefreshParty()
        {
            Send("PCK" + Leader.Character.Nickname);
            Send("PL" + Leader.Character.ID);
            Send("PM" + PartyPattern);
        }

        public void RefreshParty(World.Network.WorldClient client)
        {
            client.Send("PCK" + Leader.Character.Nickname);
            client.Send("PL" + Leader.Character.ID);
            client.Send("PM" + PartyPattern);
        }

        public void AddMember(World.Network.WorldClient client)
        {
            Send("PM+" + client.Character.Pattern.ShowCharacterInParty);
            Members.Add(client);
            RefreshParty(client);
        }

        public void RemoveMember(World.Network.WorldClient client)
        {          
            Members.Remove(client);
            client.Character.Party = null;
            Send("PM-" + client.Character.ID);
            if (Members.Count <= 1)
            {
                KickAllMembers();
                return;
            }
            if (Leader.Character.ID == client.Character.ID)
            {
                SelectRandomOwner();
            }
            Send("PL" + Leader.Character.ID);
        }

        public void KickAllMembers()
        {
            foreach (World.Network.WorldClient client in Members)
            {
                if (client != null)
                {
                    client.Character.Party = null;
                    client.Send("PV" + client.Character.ID);
                    Send("PM-" + client.Character.ID);
                }
            }
        }

        public void SelectRandomOwner()
        {
            Leader = Members[Utilities.Basic.Rand(0, Members.Count - 1)];
        }

        public void Send(string packet)
        {
            Members.ForEach(x => x.Send(packet));
        }

        public string PartyPattern
        {
            get
            {
                string pattern = "+";
                Members.ForEach(x => pattern += x.Character.Pattern.ShowCharacterInParty + "|");
                return pattern.Substring(0, pattern.Length -1);
            }
        }
    }
}
