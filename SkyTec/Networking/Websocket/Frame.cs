using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    public enum FrameType
    {
        Continuation,
        Text,
        Binary,
        ConnectionClose,
        Ping,
        Pong,
        Unknown
    }

    public struct Frame
    {
        public FrameType Type;
        public int Length;
        public bool Masked;
        public byte[] Content;
    }
}
