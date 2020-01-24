using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SuperApp.Client
{
    public class ChatQueue
    {
        private readonly Channel<Chat> _incomingMessage;
        public event Func<Chat, Task> Changed;
        public ChatQueue()
        {
            _incomingMessage = Channel.CreateUnbounded<Chat>();
        }
        public bool TryReceiveMessage(out Chat message)
        {
            if (_incomingMessage.Reader.TryRead(out message))
            {
                OnChange(message);

                return true;
            }

            return false;
        }

        private void OnChange(Chat message)
        {
            Changed.Invoke(message);
        }
    }

    
}
