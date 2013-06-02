using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    public class Connection
    {
        public event EventHandler<MessageEventArgs> onMessage;

        private Socket ClientSocket;
        private NetworkStream ClientStream;

        private WebSocketFrameReader Reader;

        internal Connection(Socket ClientSocket, NetworkStream ClientStream)
        {
            this.ClientSocket = ClientSocket;
            this.ClientStream = ClientStream;

            this.Reader = new WebSocketFrameReader(ClientStream);
        }

        internal async void BeginListening()
        {
            while (true)
            {
                Frame Frame = await this.Reader.Read();
                MessageEventArgs Message = new MessageEventArgs();
                Message.Message = Encoding.UTF8.GetString(Frame.Content);
                EventHandler<MessageEventArgs> handler = onMessage;

                if (handler == null)
                    continue;

                handler(this, Message);
            }
        }

    }
}
