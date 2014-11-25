using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Handlers
{
    public static class GuildHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("gC", typeof(GuildHandler).GetMethod("CreateGuildRequest"));
            Network.Dispatcher.RegisteredMethods.Add("gIM", typeof(GuildHandler).GetMethod("RequestMembersInfos"));
            Network.Dispatcher.RegisteredMethods.Add("gJR", typeof(GuildHandler).GetMethod("RequestInviteMember"));
            Network.Dispatcher.RegisteredMethods.Add("gJE", typeof(GuildHandler).GetMethod("RefuseInvitation"));
            Network.Dispatcher.RegisteredMethods.Add("gJK", typeof(GuildHandler).GetMethod("AcceptInvitation"));
            Network.Dispatcher.RegisteredMethods.Add("gP", typeof(GuildHandler).GetMethod("GuildPromote"));
            Network.Dispatcher.RegisteredMethods.Add("gK", typeof(GuildHandler).GetMethod("GuildKick"));
        }

        public static void CreateGuildRequest(World.Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');

            //Guild creator request pattern : gC8|16777215|8|8796216|Nightteam

            int backArtID = int.Parse(data[0]);
            int backColor = int.Parse(data[1]);
            int frontArtID = int.Parse(data[2]);
            int frontColor = int.Parse(data[3]);

            Game.Guilds.GuildEmblem emblem = new Game.Guilds.GuildEmblem(backArtID, backColor, frontArtID, frontColor);

            string guildName = data[4];

            if (!Helper.GuildHelper.ExistGuild(guildName.ToLower()))
            {
                Game.Guilds.Guild guild = new Game.Guilds.Guild(1, guildName, emblem);
                guild.AddMember(client);
                client.Action.GuildMember.Rank = Game.Guilds.GuildRank.Leader;
                client.Action.GuildMember.AllowFullRight();
            }
            else // Guild already exist
            {
                client.Action.SystemMessage("Ce nom de guilde existe deja !");
                client.Send("gV");
            }

            client.Send("gV");
        }

        public static void RequestMembersInfos(World.Network.WorldClient client, string packet = "")
        {
            if (client.Action.Guild != null)
            {
                client.Action.Guild.SendGuildMembersInformations(client);
            }
        }

        public static void RequestInviteMember(World.Network.WorldClient client, string packet)
        {
            // On verifie si il a une guilde et qu'il n'invite déjà personne
            if (client.Action.Guild != null && client.Action.InvitedGuildPlayer == -1)
            {
                if (client.Action.GuildMember != null)
                {
                    if (client.Action.GuildMember.HaveRight(Game.Guilds.GuildRightsConstants.CAN_INVITE))
                    {
                        var invitedPlayer = World.Helper.WorldHelper.GetClientByCharacter(packet.Substring(3));
                        //On verifie si le joueur voulus est connecter
                        if (invitedPlayer != null)
                        {
                            //Si il a pas de guilde on lui envois la demande
                            if (invitedPlayer.Action.Guild == null)
                            {
                                client.Send("gJR" + invitedPlayer.Character.Nickname);
                                invitedPlayer.Send("gJr" + client.Character.ID + "|" + client.Character.Nickname + "|" + client.Action.Guild.Name);

                                //On initialise les ID des demandes
                                client.Action.InvitedGuildPlayer = invitedPlayer.Character.ID;
                                invitedPlayer.Action.InvitedGuildPlayer = client.Character.ID;
                            }
                            else
                            {
                                client.Action.SystemMessage("Ce joueur possede deja une guilde !");
                            }
                        }
                    }
                    else
                    {
                        client.Action.SystemMessage("Vous ne disposer pas des droits requis !");
                    }
                }
                else
                {
                    client.Action.SystemMessage("Joueur introuvable !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous devez posseder une guilde pour effectuer cette action !");
            }
        }

        public static void CancelInvitation(World.Network.WorldClient client)
        {
            if (client != null)
            {
                if (client.Action.InvitedGuildPlayer != -1)
                {
                    var otherPlayer = World.Helper.WorldHelper.GetClientByCharacter(client.Action.InvitedGuildPlayer);
                    if (otherPlayer != null)
                    {
                        otherPlayer.Send("gJEc");
                        otherPlayer.Action.InvitedGuildPlayer = -1;
                    }
                    client.Send("gJEc");
                    client.Action.InvitedGuildPlayer = -1;
                }
            }
        }

        public static void RefuseInvitation(World.Network.WorldClient client, string packet)
        {
            if (client.Action.InvitedGuildPlayer != -1)
            {
                CancelInvitation(client);
            }
        }

        public static void AcceptInvitation(World.Network.WorldClient client, string packet)
        {
            if (client.Action.InvitedGuildPlayer != -1)
            {
                var guildPlayer = World.Helper.WorldHelper.GetClientByCharacter(client.Action.InvitedGuildPlayer);
                if (guildPlayer != null)
                {
                    if (guildPlayer.Action.Guild != null)
                    {
                        guildPlayer.Action.Guild.AddMember(client);
                        client.Action.SystemMessage("Vous avez rejoint la guilde <b>" + client.Action.Guild.Name + "</b>");
                    }
                    else
                    {
                        client.Action.SystemMessage("L'invitation a expirer !");
                    }
                    CancelInvitation(guildPlayer);
                }
                else
                {
                    client.Action.SystemMessage("L'invitation a expirer !");
                }
                CancelInvitation(client);
            }
        }

        public static void GuildPromote(World.Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');
            int id = int.Parse(data[0]);
            int rank = int.Parse(data[1]);
            int percentExp = int.Parse(data[2]);
            int rights = int.Parse(data[3]);

            if (client.Action.Guild != null && client.Action.GuildMember != null)
            {
                Game.Guilds.GuildMember guildMember = client.Action.Guild.FindMember(id);
                if (guildMember != null)
                {
                    /* Si il a le droit de modifier les rangs */
                    if (client.Action.GuildMember.HaveRight(Game.Guilds.GuildRightsConstants.CAN_SET_RANK))
                    {
                        if (guildMember.Rank != Game.Guilds.GuildRank.Leader)
                        {
                            /* On verifie si il veut pas voler la place de meneur :) */
                            if (rank != (int)Game.Guilds.GuildRank.Leader)
                            {
                                guildMember.Rank = (Game.Guilds.GuildRank)rank;
                                RequestMembersInfos(client);
                            }
                        }
                        if (client.Action.GuildMember.Rank == Game.Guilds.GuildRank.Leader)
                        {
                            if ((Game.Guilds.GuildRank)rank == Game.Guilds.GuildRank.Leader)
                            {
                                client.Action.SystemMessage("Vous n'etes desormais pu le meneur de la guilde !");
                                client.Action.GuildMember.Rank = Game.Guilds.GuildRank.InTry;
                                guildMember.Rank = Game.Guilds.GuildRank.Leader;
                                guildMember.AllowFullRight();
                                client.Action.GuildMember.Rights.Clear();
                                client.Action.SaveCharacter();
                                guildMember.Save();
                                RequestMembersInfos(client);
                            }
                        }
                    }

                    /* Si il a le droit de modifier les droits */
                    if (client.Action.GuildMember.HaveRight(Game.Guilds.GuildRightsConstants.CAN_SET_RIGHTS))
                    {
                        if (guildMember.Rank != Game.Guilds.GuildRank.Leader)
                        {
                            guildMember.Rights = guildMember.GetRightsByInt(rights);
                        }
                    }
                }
                else
                {
                    client.Action.SystemMessage("Le joueur n'existe pas, ou ne fait pas partie de la guilde !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous devez posseder une guilde pour effectuer cette action !");
            }
        }

        public static void GuildKick(World.Network.WorldClient client, string packet)
        {
            if (client.Action.Guild != null && client.Action.GuildMember != null)
            {
                string nickname = packet.Substring(2);
                Game.Guilds.GuildMember guildMember = client.Action.Guild.FindMember(nickname);
                var guild = guildMember.OwnGuild;
                if (guildMember != null)
                {
                    if (guildMember.Rank == Game.Guilds.GuildRank.Leader)
                    {
                        if (guildMember.Character.ID == client.Character.ID)
                        {
                            if (guild.Members.Count > 1)
                            {
                                client.Action.Guild.KickMember(guildMember);
                                client.Send("gKK" + client.Character.Nickname + "|" + client.Character.Nickname);
                                client.Action.Guild = null;
                                client.Action.GuildMember = null;
                                client.Action.RefreshRoleplayEntity();
                                guild.Delete();
                            }
                            else
                            {
                                client.Action.SystemMessage("Selectionner avant un nouveau meneur !");
                            }            
                            return;
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de bannir le joueur");
                            return;
                        }
                    }
                    /* Si il s'auto-kick */
                    if (guildMember.ID == client.Action.GuildMember.ID)
                    {
                        client.Action.Guild.KickMember(guildMember);
                        client.Send("gKK" + client.Character.Nickname + "|" + client.Character.Nickname);
                        client.Action.Guild = null;
                        client.Action.GuildMember = null;
                        client.Action.RefreshRoleplayEntity();
                    }
                    else /* Il kick un autre joueur que sois même */
                    {
                        if (client.Action.GuildMember.HaveRight(Game.Guilds.GuildRightsConstants.CAN_KICK))
                        {
                            client.Action.Guild.KickMember(guildMember);
                            Network.WorldClient kickedClient = World.Helper.WorldHelper.GetClientByCharacter(nickname);
                            if (kickedClient != null)
                            {
                                kickedClient.Send("gKK" + kickedClient.Character.Nickname + "|" + kickedClient.Character.Nickname);
                                kickedClient.Action.Guild = null;
                                kickedClient.Action.GuildMember = null;
                                kickedClient.Action.RefreshRoleplayEntity();
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de bannir le joueur : Vous ne posseder pas le droit");
                        }
                    }
                }
                else
                {
                    client.Action.SystemMessage("Le joueur n'existe pas, ou ne fait pas partie de la guilde !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous devez posseder une guilde pour effectuer cette action !");
            }
        }
    }
}
