using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Elite
{
    public static class EliteManager
    {
        public static void UpElite(Network.WorldClient client)
        {
            var newElite = GetNextElite(client.Character.EliteLevel);
            if (newElite != null)
            {
                client.Character.EliteLevel = newElite.Level;
                client.Character.Level = 200;
                Database.Records.ExpFloorRecord floor = Helper.ExpFloorHelper.GetCharactersLevelFloor(200);
                client.Character.Experience = floor.Character;
                client.Action.TryLevelUp();
                client.Character.TitleID = newElite.TitleID;
                client.Action.SaveCharacter();
                client.Action.RefreshRoleplayEntity();
                client.Action.SystemMessage("Vous etes desormais <b>" + newElite.Name + "</b> !");
                Manager.WorldManager.SendMessage("Le joueur <b>" + client.Character.Nickname + "</b> est monter a l'elite <b>" + newElite.Name + "</b>, felicitation a lui !", "#CD5C5C");
            }
            else
            {
                client.Action.SystemMessage("Il n'y aucun elite superieur au votre !");
            }
        }

        public static Database.Records.ElitesRecord GetNextElite(int elite)
        {
            if (Database.Cache.ElitesCache.Cache.FindAll(x => x.Level == elite + 1).Count > 0)
            {
                return Database.Cache.ElitesCache.Cache.FindAll(x => x.Level == elite + 1).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
