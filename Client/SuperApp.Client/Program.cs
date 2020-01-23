using System;
using SuperApp;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;

namespace SuperApp.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:50051");
            var client = new Chatter.ChatterClient(channel);
            Console.WriteLine("Client created");
            Console.WriteLine("Username: ");
            var userName = Console.ReadLine();

            // while(true){
            //     var message = Console.ReadLine();
                
            //     if(message == "exit"){
            //         break;
            //     }

            // }

            using (var call = client.sendMessage())
            {
                var responseTask = Task.Run(async () =>
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine($"{message.User}: {message.Message}");
                    }
                });

                while (true)
                {
                    var message = Console.ReadLine();
                    
                    if(message == "exit"){
                        break;
                    }
                    await call.RequestStream.WriteAsync(new Chat(){User = userName, Message = message});
                    // await call .RequestStream.WriteAsync(new Chat() {
                    //     message = result;
                    // };
                }

                Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
                await responseTask;
            }

            Console.WriteLine("Disconnected. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
