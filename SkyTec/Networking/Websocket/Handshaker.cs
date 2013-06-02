using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    class Handshaker
    {
        const string GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        const string Terminal = "";
        static SHA1 sha1 = SHA1CryptoServiceProvider.Create();

        public async Task<bool> HandshakeConnection(NetworkStream ClientStream)
        {
            StreamReader Reader = new StreamReader(ClientStream);
            StreamWriter Writer = new StreamWriter(ClientStream);

            TCPGetRequestHeader RequestHeader = new TCPGetRequestHeader();

            string message = await Reader.ReadLineAsync();
            while (message != Terminal)
            {
                this.ParseHeader(message, ref RequestHeader);
                message = await Reader.ReadLineAsync();
            }

            string accept = this.AcceptKey(RequestHeader.SocketKey);

            Writer.Write("HTTP/1.1 101 Switching Protocols\r\n");
            Writer.Write("Upgrade: websocket\r\n");
            Writer.Write("Connection: Upgrade\r\n");
            Writer.Write("Sec-WebSocket-Accept: " + accept + "\r\n");
            Writer.Write("\r\n");

            Writer.Flush();

            return true;
        }

        private void ParseHeader(string Header, ref TCPGetRequestHeader RequestHeader)
        {
            int length;

            length = 3;
            if (Header.Length > length && Header.Substring(0, length) == "GET")
                RequestHeader.Method = Header;

            length = 7;
            if (Header.Length > length && Header.Substring(0, length) == "Upgrade")
                RequestHeader.Upgrade = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 10;
            if (Header.Length > length && Header.Substring(0, length) == "Connection")
                RequestHeader.Connection = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 4;
            if (Header.Length > length && Header.Substring(0, length) == "Host")
                RequestHeader.Host = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 6;
            if (Header.Length > length && Header.Substring(0, length) == "Origin")
                RequestHeader.Origin = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 10;
            if (Header.Length > length && Header.Substring(0, length) == "User-Agent")
                RequestHeader.UserAgent = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 6;
            if (Header.Length > length && Header.Substring(0, length) == "Pragma")
                RequestHeader.Pragma = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 13;
            if (Header.Length > length && Header.Substring(0, length) == "Cache-Control")
                RequestHeader.CacheControl = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 17;
            if (Header.Length > length && Header.Substring(0, length) == "Sec-WebSocket-Key")
                RequestHeader.SocketKey = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 21;
            if (Header.Length > length && Header.Substring(0, length) == "Sec-WebSocket-Version")
                RequestHeader.SocketVersion = Header.Substring(length + 2, Header.Length - (length + 2));

            length = 24;
            if (Header.Length > length && Header.Substring(0, length) == "Sec-WebSocket-Extensions")
                RequestHeader.SocketExtensions = Header.Substring(length + 2, Header.Length - (length + 2));
        }

        private string AcceptKey(string key)
        {
            string longKey = key + GUID;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        private static byte[] ComputeHash(string str)
        {
            return sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }

    }
}
