using System;
using SuperApp;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;

namespace SuperApp.Client
{
    class Program
    {
        private static List<Chat> Chats = new List<Chat>();
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:50051");

            var Client = new Chatter.ChatterClient(channel);

            Console.WriteLine("Client created");
            Console.Write("Username: ");
            var User = Console.ReadLine();

            var message = new Chat()
            {
                User = User,
                Message = "I has entered the building!"
            };

            using (var call = Client.sendMessage())
            {
                var responseTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        Chats.Add(call.ResponseStream.Current);
                        Console.Clear();
                        foreach (var chat in Chats)
                        {
                            Console.WriteLine($"{message.User}: {message.Message}");
                        }
                    }
                });

                await call.RequestStream.WriteAsync(message);

                while (message.Message != string.Empty)
                {
                    Console.Write("Send: ");
                    message.Message = Console.ReadLine();
                    await call.RequestStream.WriteAsync(message);
                }

                Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnected. Press any key to exit.");
            Console.ReadKey();
        }

        private static Task ReceiveMessageHandler(Chat arg)
        {
            throw new NotImplementedException();
        }

        // private static async Task MessageRecievedHandler(Chat message)
        // {
        //     Chats.Add(message);
        //     Console.Clear();
        //     foreach (var msg in Chats)
        //     {
        //         Console.WriteLine($"{message.User}: {message.Message}");
        //     }

        //     Console.Write("Send: ");
        //     var sendMessage = Console.ReadLine();
        //     await SendMessage(message);
        // }

        // private static async Task SendMessage(Chat message)
        // {
        //     await Call.RequestStream.WriteAsync(message);

        // }
    }
}
