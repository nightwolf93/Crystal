using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : FightDrop
*/

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightDrops
    {
        public Fighter Dropper { get; set; }
        public Dictionary<int, int> Drops = new Dictionary<int,int>();

        public FightDrops(Fighter dropper)
        {
            this.Dropper = dropper;
        }

        public void AddDrop(int itemID)
        {
            if (this.Drops.ContainsKey(itemID))
            {
                this.Drops[itemID]++;
            }
            else
            {
                this.Drops.Add(itemID, 1);
            }
        }

        public void GenerateInInventory()
        {
            foreach (var drop in this.Drops)
            {
                Dropper.Client.Character.Items.AddItem(Helper.ItemHelper.GenerateItem(Dropper.Client, drop.Key), false, drop.Value);
            }
        }

        public string Parse
        {
            get
            {
                StringBuilder packet = new StringBuilder();
                foreach (var drop in Drops)
                {
                    if (packet.ToString().Length > 0) packet.Append(",");
                    packet.Append(drop.Key);
                    packet.Append("~");
                    packet.Append(drop.Value);
                }
                return packet.ToString();
            }
        }
    }
}
