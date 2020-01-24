using System;
using System.Threading.Tasks;
using Grpc.Core;
using SuperApp;


namespace SuperApp.Server
{
    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { Chatter.BindService(new ChatService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Chatter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.Read();

            server.ShutdownAsync().Wait();

        }
    }
}
