using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Patterns
{
    public class NpcPattern
    {
        private Database.Records.NpcPositionRecord _npc = null;

        public NpcPattern(Database.Records.NpcPositionRecord npc)
        {
            this._npc = npc;    
        }

        public string DisplayOnMap
        {
            get
            {
                try
                {
                    return "|+" + _npc.CellId + ";" + _npc.Orientation + ";0;" + _npc.TempID + ";" + _npc.Template.ID +
                        ";-4;" + _npc.Template.Gfx + "^" + _npc.Template.ScaleX + ";" + _npc.Template.Sex +
                        ";" + _npc.Template.Color1.ToString("x") + ";" + _npc.Template.Color2.ToString("x") + ";" + _npc.Template.Color3.ToString("x") +
                        ";" + _npc.Template.Accessories + ";;" + _npc.Template.ArtWork;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }

        public string SellPattern
        {
            get
            {
                if (this._npc != null)
                {
                    if (this._npc.Template.SaleItems != "-1" && this._npc.Template.SaleItems != "" && this._npc.Template.SaleItems != null)
                    {
                        List<string> data = this._npc.Template.SaleItems.Split(',').ToList();
                        string pattern = "";
                        foreach (string x in data)
                        {
                            try
                            {
                                if (x != "" && x != null)
                                {
                                    Database.Records.ItemRecord item = Database.Cache.ItemCache.Cache.FirstOrDefault(y => y.ID == int.Parse(x));
                                    if (item != null)
                                    {
                                        pattern += item.ID + ";" + item.Engine.StringEffect() + "|";
                                    }
                                }
                            }
                            catch { }               
                        }
                        return pattern;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
