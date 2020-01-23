using SuperApp;
using System.Threading.Tasks;
using Grpc.Core;
using System;

namespace SuperApp.Server
{


    public class ChatService : Chatter.ChatterBase
    {
        private static readonly ChatQueue _queue = new ChatQueue();
        public async override Task sendMessage(IAsyncStreamReader<Chat> requestStream, IServerStreamWriter<Chat> responseStream, Grpc.Core.ServerCallContext context)
        {
            _queue.Changed += SendMessageHandler;
            try
            {
                while (await requestStream.MoveNext())
                {
                    _queue.TrySendMessage(out var message);
                }
            }
            finally
            {
                _queue.Changed -= SendMessageHandler;
            }

            async Task SendMessageHandler(Chat message)
            {
                await responseStream.WriteAsync(message);
            }
        }
    }
}