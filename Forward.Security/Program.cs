using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Forward.Security
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Forward security manager");

            while (true)
            {
                string[] command = Console.ReadLine().Split(' ');
                switch (command[0])
                {
                    case "genkey":
                        string key = GenerateKey(1024);
                        Console.WriteLine("Generated key : " + key);
                        StreamWriter keyWriter = new StreamWriter("genkey.txt");
                        keyWriter.Write(key);
                        keyWriter.Close();
                        break;

                    case "encryptsha512":
                        Console.WriteLine(SHA512.GetSHA512(command[1]));
                        StreamWriter shaWriter = new StreamWriter("sha512encryption.txt");
                        shaWriter.Write(SHA512.GetSHA512(command[1]));
                        shaWriter.Close();
                        break;
                }
            }
        }

        public static string GenerateKey(int lenght)
        {
            string hash = "azertyuiopqsdfghjklmwxcvbn-_123456789";
            string key = "fk_";
            Random rand = new Random(Environment.TickCount);
            for (int i = 0; i <= lenght - 1; i++)
            {
                key += hash[rand.Next(0, hash.Length -1)];
            }
            return key;
        }
    }
}
