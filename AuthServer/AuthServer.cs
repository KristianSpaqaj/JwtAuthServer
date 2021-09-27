using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AuthServer
{
    class AuthServer
    {
        public List<string> DictionaryWords { get; set; } = new List<string>();
        public Dictionary<string, string> UserInformations { get; set; } = new Dictionary<string, string>();

        public void Listen(int port)
        {
            ReadDictionaryWords();
            ReadUserInformations();
            //TcpListener serverSocket = new TcpListener(port);
            TcpListener server = new TcpListener(IPAddress.Loopback, port);
            server.Start();
            Console.WriteLine("Server listning on port: " + port);

            TcpClient connectionSocket = server.AcceptTcpClient();
            Console.WriteLine("Server activated");

            Stream ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true; // enable automatic flushing

            sw.WriteLine(JsonConvert.SerializeObject(UserInformations));
            sw.WriteLine(JsonConvert.SerializeObject(DictionaryWords.Take(10000)));

            Console.WriteLine("Press Enter to Close");
            Console.ReadKey();
            ns.Close();
            connectionSocket.Close();
            server.Stop();

        }

        private void ReadDictionaryWords()
        {
            string line;
            StreamReader file = new StreamReader(@"C:\Users\sunsh\Desktop\4.semester\IT-Sikkerhed\JwtAuthServerClient\JwtAuthServer\webster-dictionary.txt");
            while ((line = file.ReadLine()) != null)
            {
                DictionaryWords.Add(line);
            }
            Console.WriteLine(DictionaryWords.Count.ToString());
        }

        private void ReadUserInformations()
        {
            string line;
            StreamReader file = new StreamReader(@"C:\Users\sunsh\Desktop\4.semester\IT-Sikkerhed\PasswordCrackerCentralized\PasswordCrackerCentralized\bin\Debug\pass.txt");
            while ((line = file.ReadLine()) != null)
            {
                List<string> tokens = line.Split(new char[] { ':' }).ToList();
                UserInformations.Add(tokens[0], tokens[1]);
            }
            Console.WriteLine(String.Join(", ", UserInformations.ToList()));
        }

        private string GenerateToken(string username)
        {
            // Define const Key this should be private secret key  stored in some safe place normally
            string key =
                "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            // Create symmetric Security key from the above string/key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256Signature);
            //  Finally create a Token
            var header = new JwtHeader(credentials);
            //Some PayLoad that contain information about the  customer
            var payload = new JwtPayload
            {
                { "name ", username},
                { "scope", "http://localhost/"},
                { "auth_time", DateTime.Now},
                { "exp", TimeSpan.FromMinutes(20)}
            };

            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return handler.WriteToken(secToken);
        }

        private User UserExists(string username, string password)
        {
            return UserStorage.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

    }
    
}
