using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    public class Server
    {
        public event EventHandler<ConnectionEventArgs> onConnection;

        public int Port
        {
            get;
            private set;
        }

        private Socket Socket;
        private Handshaker Handshaker;

        public Server(int Port)
        {
            this.Port = Port;
        }

        public void Start()
        {
            this.Port = Port;
            this.Handshaker = new Handshaker();

            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            this.Socket.Bind(new IPEndPoint(IPAddress.Any, this.Port));
            this.Socket.Listen(128);
            this.Socket.BeginAccept(null, 0, AcceptConnection, null);
        }

        private void AcceptConnection(IAsyncResult AsyncResult)
        {
            Socket Client;

            try
            {
                Client = this.Socket.EndAccept(AsyncResult);
                this.ProcessConnection(Client);
            }
            catch (SocketException exception)
            {
                Debug.WriteLine(exception.Message);
            }
            finally
            {
                if (this.Socket != null && this.Socket.IsBound)
                {
                    this.Socket.BeginAccept(null, 0, AcceptConnection, null);
                }
            }
        }

        private async void ProcessConnection(Socket Client)
        {
            NetworkStream ClientStream = new NetworkStream(Client);

            if (await this.Handshaker.HandshakeConnection(ClientStream))
            {
                this.FinalizeConnection(Client, ClientStream);
            }
            else
            {
                //find propper way to disconnect
                Client.Disconnect(false);
            }
        }

        private void FinalizeConnection(Socket ClientSocket, NetworkStream ClientStream)
        {
            Connection Connection = new Connection(ClientSocket, ClientStream);

            var Message = new ConnectionEventArgs();
            Message.Connection = Connection;

            EventHandler<ConnectionEventArgs> handler = onConnection;

            if (handler == null)
                return;

            handler(this, Message);

            Connection.BeginListening();
        }

    }
}
