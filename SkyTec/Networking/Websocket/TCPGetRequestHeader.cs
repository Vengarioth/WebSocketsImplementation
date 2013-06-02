using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    struct TCPGetRequestHeader
    {
        public string Method; //"GET /websession HTTP/1.1"
        public string Upgrade; //"Upgrade: websocket"
        public string Connection; //"Connection: Upgrade"

        public string Host; //"Host: localhost:8080"
        public string Origin; //"Origin: http://localhost3000"
        public string UserAgent; //"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36"

        public string Pragma; //"Pragma: no-cache"
        public string CacheControl; //"Cache-Control: no-cache"

        public string SocketKey; //"Sec-WebSocket-Key: ODyqsL3u5Y2L/diDsdxMAQ=="
        public string SocketVersion; //"Sec-WebSocket-Version: 13"
        public string SocketExtensions; //"Sec-WebSocket-Extensions: x-webkit-deflate-frame"
    }
}
