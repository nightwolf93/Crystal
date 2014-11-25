using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class FriendHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("FA", typeof(FriendHandler).GetMethod("AddFriend"));
            Network.Dispatcher.RegisteredMethods.Add("FD", typeof(FriendHandler).GetMethod("DeleteFriend"));
            Network.Dispatcher.RegisteredMethods.Add("FL", typeof(FriendHandler).GetMethod("ShowFriends"));
        }

        public static void ShowFriends(World.Network.WorldClient client, string packet = "")
        {
            string friendPacket = "FL";
            foreach (int i in client.AccountData.FriendsIDs)
            {
                if (Helper.AccountHelper.ExistAccountData(i))
                {
                    Database.Records.AccountDataRecord account = Helper.AccountHelper.GetAccountData(i);
                    friendPacket += "|" + account.NickName;
                    World.Network.WorldClient player = Helper.WorldHelper.GetClientByAccountNickName(account.NickName);
                    if (player != null)
                    {
                        if (player.AccountData.FriendsIDs.Contains(client.AccountData.AccountID))
                        {
                            friendPacket += player.Character.Pattern.CharacterToFriendsListKnow;
                        }
                        else
                        {
                            friendPacket += player.Character.Pattern.CharacterToFriendsListUnKnow;
                        }
                    }
                }
            }
            client.Send(friendPacket);
        }

        public static void AddFriend(World.Network.WorldClient client, string packet)
        {
            string addType = packet[2].ToString();
            string nickname;
            Network.WorldClient player;
            switch (addType)
            {
                case "%"://Character name
                    nickname = packet.Substring(3);
                    player = Helper.WorldHelper.GetClientByCharacter(nickname);
                    if (player != null)
                    {
                        client.AccountData.FriendsIDs.Add(player.AccountData.AccountID);
                    }
                    else
                    {
                        client.Send("cMEf" + nickname);
                    }
                    break;

                case "*":
                    nickname = packet.Substring(3);
                    player = Helper.WorldHelper.GetClientByCharacter(nickname);
                    if (player != null)
                    {
                        client.AccountData.FriendsIDs.Add(player.AccountData.AccountID);
                    }
                    else
                    {
                        client.Send("cMEf" + nickname);
                    }
                    break;

                default:
                    nickname = packet.Substring(2);
                    player = Helper.WorldHelper.GetClientByCharacter(nickname);
                    if (player != null)
                    {
                        client.AccountData.FriendsIDs.Add(player.AccountData.AccountID);
                        client.Send("BN");
                    }
                    else
                    {
                        client.Send("cMEf" + nickname);
                    }
                    break;
            }
        }

        public static void DeleteFriend(World.Network.WorldClient client, string packet)
        {
            string nickname = packet.Substring(3);
            if (Helper.AccountHelper.ExistAccountData(nickname))
            {
                Database.Records.AccountDataRecord account = Helper.AccountHelper.GetAccountData(nickname);
                if (client.AccountData.FriendsIDs.Contains(account.AccountID))
                {
                    client.AccountData.FriendsIDs.Remove(account.AccountID);
                    ShowFriends(client);
                }
            }
        }

        public static void WarnConnectionToFriends(World.Network.WorldClient client)
        {
            foreach (int i in client.AccountData.FriendsIDs)
            {
                if (Helper.AccountHelper.ExistAccountData(i))
                {
                    Database.Records.AccountDataRecord account = Helper.AccountHelper.GetAccountData(i);
                    World.Network.WorldClient player = Helper.WorldHelper.GetClientByAccountNickName(account.NickName);
                    if (player != null)
                    {
                        if (player.AccountData.FriendsIDs.Contains(client.AccountData.AccountID))
                        {
                            player.SendImPacket("0143", client.AccountData.NickName + 
                                        "(<b><a href='asfunction:onHref,ShowPlayerPopupMenu," + 
                                        client.Character.Nickname + "'>" + client.Character.Nickname + "</a></b>)");
                        }
                    }
                }
            }
        }
    }
}
