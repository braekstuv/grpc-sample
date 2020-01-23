using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SuperApp.Server
{
    public class ChatQueue
    {
        private int _totalSentMessages;
        private readonly Channel<Chat> _incomingMessage;
        public event Func<Chat, Task> Changed;
        public ChatQueue()
        {
            _incomingMessage = Channel.CreateUnbounded<Chat>();

            // Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         _totalSentMessages++;
            //         var mail = new Chat(){
            //             User = 
            //             Message =
            //         };
            //         await _incomingMessage.Writer.WriteAsync(mail);
            //         OnChange(MailboxMessage.Types.Reason.Received);

            //         await Task.Delay(TimeSpan.FromSeconds(random.Next(5, 15)));
            //     }
            // });
        }
        public bool TrySendMessage(out Chat message)
        {
            if (_incomingMessage.Reader.TryRead(out message))
            {
                Interlocked.Increment(ref _totalSentMessages);
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
