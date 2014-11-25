using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class CharacterCache
    {
        public static List<Records.CharacterRecord> Cache = new List<Records.CharacterRecord>();

        public static void Init()
        {
            Cache = Records.CharacterRecord.FindAll().ToList();
            foreach (var character in Cache)
            {
                if (character.GuildID != 0)
                {
                    try
                    {
                        var guild = World.Helper.GuildHelper.GetGuild(character.GuildID);
                        if (guild != null)
                        {
                            var member = new World.Game.Guilds.GuildMember(character, guild);
                            guild.Members.Add(member);
                        }
                    }
                    catch (Exception e) { }
                }
                if (character.MountID != 0)
                {
                    try
                    {
                        var mount = World.Helper.MountHelper.GetMountByID(character.MountID);
                        if (mount != null)
                        {
                            character.Mount = mount;
                        }
                    }
                    catch (Exception e) { }
                }
            }
        }

        public static void SetToMaxLife()
        {
            foreach (var character in Cache)
            {
                character.CurrentLife = character.Stats.MaxLife;
            }
        }
    }
}
