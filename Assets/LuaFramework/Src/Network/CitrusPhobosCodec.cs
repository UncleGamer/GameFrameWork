using CitrusNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace LuaFramework
{
    
    /// <summary>
    /// 编码解码器
    /// 和网络库中不同的是，游戏中需要兼容lua, 会将数据存储在ByteNetCommand中，后面传递给Lua
    /// </summary>
    public class CitrusPhobosCodec : CitrusNet.Phobos.PhobosExCodec
    {

        protected override uint Getpid(NetCommand cmd)
        {
            return cmd.GetType() == typeof(ByteNetCommand) ? (cmd as ByteNetCommand).pid : Registry.MustFind(cmd.GetType());
        }
        //解码
        protected override NetCommand DecodeCmd(uint pid, MemoryStream payloadStream)
        {
            NetCommand cmd = null;
            // 根据pid找到对应的消息对象
            var msgType = Registry.Find((uint)pid);
            if (msgType == null)
            {
                cmd = new ByteNetCommand { pid = (uint)pid, buf = payloadStream.ToArray() };
            }
            else
            {
                cmd = ProtoBuf.Serializer.Deserialize(msgType, payloadStream) as NetCommand;
            }
            return cmd;
        }
        //编码
        protected override byte[] EncodeCmd(NetCommand cmd)
        {
            if(cmd.GetType() == typeof(ByteNetCommand))
            {
                return (cmd as ByteNetCommand).buf;
            }
            else
            {
                var pyaloadStream = new MemoryStream();
                ProtoBuf.Serializer.Serialize(pyaloadStream, cmd);
                return pyaloadStream.ToArray();
            }
        }
    }

}