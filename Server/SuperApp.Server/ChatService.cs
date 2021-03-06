using SuperApp;
using System.Threading.Tasks;
using Grpc.Core;
using System;

namespace SuperApp.Server
{
    public class ChatService : Chatter.ChatterBase
    {
        private static readonly ChatQueue _queue = new ChatQueue();

        public override Task<HelloWorldResult> HelloWorld(HelloWorldRequest request, ServerCallContext context){
            return Task.FromResult(new HelloWorldResult(){
                Message = $"Hello {request.Name}!"
            });
        }

        public async override Task SendMessage(IAsyncStreamReader<Chat> requestStream, IServerStreamWriter<Chat> responseStream, Grpc.Core.ServerCallContext context)
        {
            Console.WriteLine("Incoming call.");
            _queue.Changed += SendMessageHandler;
            try
            {
                while (await requestStream.MoveNext())
                {
                    Console.WriteLine("Incoming message.");
                    _queue.TrySendMessage(out var message);
                }
            }
            finally
            {
                Console.WriteLine("Call closing.");
                _queue.Changed -= SendMessageHandler;
            }

            async Task SendMessageHandler(Chat message)
            {
                await responseStream.WriteAsync(message);
            }
        }
    }
}