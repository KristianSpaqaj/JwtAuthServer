using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer
{
    class AuthServer
    {

        public void Listen(int port)
        {
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
            
            string request = sr.ReadLine();

            while (!string.IsNullOrEmpty(request))
            {
                if (request.IndexOf(':') != -1)
                {
                    Console.WriteLine("User credentials: " + request);
                    var data = request.Split(':');
                    if (UserExists(data[0], data[1]) != null)
                    {
                        var token = GenerateToken(data[0]);
                        sw.WriteLine(token);
                    }
                    else
                    {
                        sw.WriteLine("Wrong username or password");
                    }
                }
                else
                {
                    sw.WriteLine("you must provide a valid username and password in this format:> [username:password]");
                }
                try
                {
                    request = sr.ReadLine();
                }
                catch (Exception e)
                {
                    request = null;
                    Console.WriteLine("Client got crazy");
                }
            }
            Console.WriteLine("Press Enter to Close");
            Console.ReadKey();
            ns.Close();
            connectionSocket.Close();
            server.Stop();

        }

        private string GenerateToken(string username)
        {
            // Define const Key this should be private secret key  stored in some safe place
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
