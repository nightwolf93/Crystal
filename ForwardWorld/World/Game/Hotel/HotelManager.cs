using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Hotel
{
    public static class HotelManager
    {
        public static bool IsFree(string name)
        {
            lock (Database.Cache.HotelCache.Cache)
            {
                if (Database.Cache.HotelCache.Cache.Count > 0)
                {
                    var room = GetRoom(name);
                    if (room != null)
                    {
                        if (room.Owner != "")
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public static Database.Records.HotelRecord GetRoom(string name)
        {
            lock (Database.Cache.HotelCache.Cache)
            {
                if (Database.Cache.HotelCache.Cache.FindAll(x => x.Name.ToLower() == name.ToLower()).Count > 0)
                    return Database.Cache.HotelCache.Cache.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                return null;
            }
        }

        public static Database.Records.HotelRecord GetRoom(int mapid)
        {
            lock (Database.Cache.HotelCache.Cache)
            {
                if (Database.Cache.HotelCache.Cache.FindAll(x => x.MapID == mapid).Count > 0)
                    return Database.Cache.HotelCache.Cache.FirstOrDefault(x => x.MapID == mapid);
                return null;
            }
        }

        public static Database.Records.HotelRecord GetRoomByOwner(string owner)
        {
            lock (Database.Cache.HotelCache.Cache)
            {
                if (Database.Cache.HotelCache.Cache.FindAll(x => x.Owner.ToLower() == owner.ToLower()).Count > 0)
                    return Database.Cache.HotelCache.Cache.FirstOrDefault(x => x.Owner.ToLower() == owner.ToLower());
                return null;
            }
        }

        public static void LocateRoom(World.Network.WorldClient client, string room)
        {
            var r = GetRoom(room);
            if (r != null)
            {
                if (IsFree(room))
                {
                    r.Owner = client.Character.Nickname;
                    r.LocateTimer = new System.Timers.Timer(3600000);
                    r.LocateTimer.Enabled = true;
                    r.LocateTimer.Elapsed += r.UnLocate;
                    World.Network.World.SendNotification("La salle '" + r.Name + "' a ete louer par '" + r.Owner + "' !");
                    GotoRoom(client, room);
                }
                else
                {
                    client.Action.SystemMessage("Cette salle est occuper pour le moment !");
                }
            }
            else
            {
                client.Action.SystemMessage("Aucune salle n'existe avec ce nom !");
            }
        }

        public static void ChangeRoomPassword(World.Network.WorldClient client, string password)
        {
            var r = GetRoom(client.Character.MapID);
            if (r != null)
            {
                if (r.Owner == client.Character.Nickname)
                {
                    r.Password = password;
                    client.Action.SystemMessage("Le mot de passe a ete changer en : <b>" + password + "</b>");
                }
                else
                {
                    client.Action.SystemMessage("Vous n'etes pas le gerant de la salle, celui qui la gere est <b>" + r.Owner + "</b> !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous n'etes dans aucune salle !");
            }
        }

        public static void CloseRoom(World.Network.WorldClient client)
        {
            var r = GetRoom(client.Character.MapID);
            if (r != null)
            {
                if (r.Owner == client.Character.Nickname)
                {
                    r.UnLocate(null, null);
                }
                else
                {
                    client.Action.SystemMessage("Vous n'etes pas le gerant de la salle, celui qui la gere est <b>" + r.Owner + "</b> !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous n'etes dans aucune salle !");
            }
        }

        public static void GotoRoom(World.Network.WorldClient client, string room, string password = "")
        {
            var r = GetRoom(room);
            if (r != null)
            {
                if (!IsFree(room))
                {
                    //Check the password
                    if (r.Password != "")
                    {
                        if (r.Password != password)
                        {
                            client.Action.SystemMessage("Le mot de passe de la salle est incorrect !");
                            return;
                        }
                    }

                    client.Action.Teleport(r.MapID, r.CellID);
                    if (client.Character.Nickname == r.Owner)
                    {
                        client.Action.SystemMessage("Bienvenue dans votre salle que <b>vous venez de louer</b>.");
                        client.Action.SystemMessage("<b>Plusieurs commandes</b> sont a votre disposition pour gerer votre salle");
                        client.Action.SystemMessage("Cette salle est disponible pendant une heure, vous serez ejecter de la salle si vous etes encore dedant apres le temps donner.");
                        client.Action.SystemMessage("<b>Liste des commandes :</b>");
                        client.Action.SystemMessage("<b>.hotel mdp (pass)</b> : Change le mot de passe de la salle");
                        client.Action.SystemMessage("<b>.hotel close</b> : Termine la location de la salle");
                    }
                    else
                    {
                        client.Action.SystemMessage("Bievenue dans la salle <b>" + r.Name + "</b>, le gerant de la salle est <b>" + r.Owner + "</b>");
                    }
                }
                else
                {
                    client.Action.SystemMessage("Vous ne pouvez pas rejoindre cette salle car elle n'est pas encore louer !");
                }
            }
            else
            {
                client.Action.SystemMessage("Aucune salle n'existe avec ce nom !");
            }
        }
    }
}
