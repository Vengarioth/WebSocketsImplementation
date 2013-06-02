using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTec.Networking.Websocket
{
    class WebSocketFrameReader
    {
        private Stream Stream;

        public WebSocketFrameReader(Stream Stream)
        {
            this.Stream = Stream;
        }

        public async Task<Frame> Read()
        {
            Frame Frame = new Frame();

            byte Type = await this.ReadSingleByte();

            if (Type == 129)
                Frame.Type = FrameType.Text;
            else if (Type == 130)
                Frame.Type = FrameType.Binary;
            else if (Type == 185)
                Frame.Type = FrameType.Unknown;
            else
                Frame.Type = FrameType.Unknown;

            byte Size = await this.ReadSingleByte();

            //TODO see if first bit is set to 0, meaning non decrypted message
            Frame.Masked = true;

            byte LengthCode = (byte)(Size & 127);

            if (LengthCode == 126)
            {
                byte[] len = await this.ReadFixedLength(2);
                Array.Reverse(len);
                ulong length = BitConverter.ToUInt16(len, 0);
                Frame.Length = (int)length;
            }
            else if (LengthCode == 127)
            {
                byte[] len = await this.ReadFixedLength(8);
                Array.Reverse(len);
                ulong length = BitConverter.ToUInt64(len, 0);
                Frame.Length = (int)length;
            }
            else
            {
                Frame.Length = LengthCode;
            }

            byte[] Masks = await this.ReadFixedLength(4);
            byte[] Data = await this.ReadFixedLength((int)Frame.Length);

            Frame.Content = this.Decode(Data, Masks);

            return Frame;
        }

        private byte[] Decode(byte[] Data, byte[] Masks)
        {
            byte[] Result = new byte[Data.Length];

            for (var i = 0; i < Data.Length; i++)
            {
                Result[i] = (byte)(Data[i] ^ Masks[i % 4]);
            }

            return Result;
        }

        private async Task<byte> ReadSingleByte()
        {
            byte[] buffer = new byte[1];
            await this.Stream.ReadAsync(buffer, 0, 1);
            return buffer[0];
        }

        private async Task<byte[]> ReadFixedLength(int length)
        {
            byte[] buffer = new byte[length];
            await this.Stream.ReadAsync(buffer, 0, length);
            return buffer;
        }
    }
}
