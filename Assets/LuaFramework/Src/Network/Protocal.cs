
namespace LuaFramework {
	public class Protocal {
		///BUILD TABLE
		public const int Connect = 101;     //连接服务器
		public const int Exception = 102;     //异常掉线
		public const int Disconnect = 103;     //正常断线   
        public const int Message = 104;     //接收消息   
    }


    //二进制交互的协议
    public class ByteNetCommand : CitrusNet.NetCommand
    {
        // 协议ID
        public uint pid { get; set; }
        // 协议名称
        public string msgname { get; set; }
        // 二进制协议数据
        public byte[] buf { get; set; }

        public ByteNetCommand() { }
        public ByteNetCommand(uint _pid, string _msgname, byte[] _buf)
        {
            pid = _pid;
            msgname = _msgname;
            buf = new byte[_buf.Length];
            System.Array.Copy(_buf, buf, _buf.Length);
        }
    }
}