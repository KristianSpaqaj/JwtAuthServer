using AuthServer;

namespace AuthServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthServer server = new AuthServer();
            server.Listen(6789);
        }
    }
}
