using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Helper
{
    public static class AccountHelper
    {
        public static Random Rand = new Random(Environment.TickCount);
        public static string[] LettersPairs = { "lo", "la", "li", "wo", "wi", "ka", "ko", "ki", "po",
                                                  "pi", "pa", "aw", "al", "na", "ni", "ny", "no", "ba", "bi",
                                                  "ra", "ri", "ze", "za", "da", "zel", "wo" };

        public static List<Database.Records.CharacterRecord> GetCharactersForOwner(int owner)
        {
            return Database.Cache.CharacterCache.Cache.FindAll(x => x.Owner == owner);
        }

        public static bool ExistAccountData(int ID)
        {
            return Database.Cache.AccountDataCache.Cache.FindAll(x => x.AccountID == ID).Count > 0;
        }

        public static bool ExistAccountData(string nickname)
        {
            return Database.Cache.AccountDataCache.Cache.FindAll(x => x.NickName == nickname).Count > 0;
        }

        public static Database.Records.AccountDataRecord GetAccountData(int ID)
        {
            return Database.Cache.AccountDataCache.Cache.FirstOrDefault(x => x.AccountID == ID);
        }

        public static Database.Records.AccountDataRecord GetAccountData(string nickname)
        {
            return Database.Cache.AccountDataCache.Cache.FirstOrDefault(x => x.NickName == nickname);
        }

        public static Database.Records.AccountDataRecord CreateNewAccountData(Database.Records.AccountRecord account)
        {
            try
            {
                Database.Records.AccountDataRecord newAccountData = new Database.Records.AccountDataRecord();
                newAccountData.AccountID = account.ID;
                newAccountData.NickName = account.Pseudo;
                newAccountData.Bank = World.Game.Items.ItemBag.CreateBag();
                newAccountData.Bank.Create();
                newAccountData.BankID = newAccountData.Bank.ID;
                Database.Cache.AccountDataCache.Cache.Add(newAccountData);
                newAccountData.CreateAndFlush();
                return newAccountData;
            }catch(Exception e)
            {
                Utilities.ConsoleStyle.Error("Error : " + e.ToString());
                return null;
            }       
        }

        public static Database.Records.CharacterRecord GetCharacter(int id)
        {
            return Database.Cache.CharacterCache.Cache.FirstOrDefault(x => x.ID == id);
        }

        public static bool ExistName(string name)
        {
            return Database.Cache.CharacterCache.Cache.FindAll(x => x.Nickname.ToLower() == name.ToLower()).Count > 0 ? true : false;
        }

        public static int GetDefaultLook(Database.Records.CharacterRecord character)
        {
            return int.Parse(character.Breed.ToString() + character.Gender.ToString());
        }

        public static int RandomNumber(int min, int max)
        {
            return Rand.Next(min, max);
        }

        public static string GetRandomName()
        {
            string Name = "";
            for (int i = 0; i <= RandomNumber(2, 4); i++)
            {
                Name += LettersPairs[RandomNumber(0, LettersPairs.Length - 1)];
            }
            Name = Name.ToCharArray()[0].ToString().ToUpper() + Name.Substring(1);
            return Name;
        }
    }
}
