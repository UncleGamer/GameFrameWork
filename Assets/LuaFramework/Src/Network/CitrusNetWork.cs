using System.Collections;
using System.Collections.Generic;
using CitrusNet;
using System;
using CitrusNet.Phobos;
using System.Threading;
namespace LuaFramework
{
    public class CitrusNetWork : PhobosNetwork<TcpTransport, CitrusPhobosCodec>, NetMetadata
    {
        private string[] mSeparator = new string[] { "," };
        public string[] Separator => mSeparator;

        public CitrusNetWork()
        {
            Metadata = this;

            this.HeartbeatInterval = 3;
            this.DetectAliveTime = 10;
        }

        private Dictionary<string, string> metas = new Dictionary<string, string>();
        private Dictionary<string, string> paths = new Dictionary<string, string>();
        private Dictionary<string, string> types = new Dictionary<string, string>();

        public void SetValue(string meta, string val)
        {
            metas.Add(meta, val);
        }
        public string GetValue(string meta, NetCommand args)
        {
            switch (meta)
            {
                case "route":
                    {
                        string tpName = args.GetType() == typeof(ByteNetCommand) ? (args as ByteNetCommand).msgname : args.GetType().Name;
                        string path = makeRouteName(tpName);
                        string tp = types[tpName];
                        return path + $"{GetValue(tp, args)}";
                    }

                default:
                    return metas[meta];
            }
        }
        private string makeRouteName(string name)
        {
            string result = "";
            if (paths.TryGetValue(name, out result))
            {
                return result;
            }
            result = "/";
            string tp = "";

            string[] sts = name.ToLower().Split('_');
            if(sts.Length > 1)
            {
                for (int i = 1; i < sts.Length; i++)
                {
                    if (i == 1) tp = sts[i];
                    result += sts[i];
                    result += "/";
                }
            }
            else
            {
                throw new Exception("not support proto name " + name);
            }
            
            paths[name] = result;
            types[name] = tp;

            return result;
        }

        private bool HandlePush_Ex(PhobosPacket packet)
        {
            //1.二进制传输的消息 直接广播出去
            var bytecmd = packet.Command.GetType() == typeof(ByteNetCommand);
            if (bytecmd)
            {
                NetworkManager.AddEvent(Protocal.Message, (ByteNetCommand)packet.Command);
                return true;
            }

            var pid = Registry.MustFind(packet.Command.GetType());

            //2. 服务器下推消息
            NetCallback callback = null;
            if (mHandlers.TryGetValue(pid, out callback))
            {
                // 调用回调函数
                callback.Invoke(packet.Head, packet.Command);
                return true;
            }

            Util.Log($"Push callback not found for: {pid}");
            return false;
        }

        //消息处理
        public override void OnMessage(Channel channel, NetPacket packet)
        {
            var phobosPacket = (PhobosPacket)packet;
            Util.LogWarning($"--< Receive  Message >--  msg: {phobosPacket.Command}   ProtoID: {phobosPacket.ProtoID}     Errno: {phobosPacket.Head.Errno}    error: {phobosPacket.Head.Error}");
            if(phobosPacket.Head.Errno != (int)errors.proto.E_Code.E_Ok)
            {
                return;
            }
            if (phobosPacket.Head.RequestId != 0)
            {
                // 处理请求回应消息
                HandleRequest(phobosPacket);
            }
            else
            {
                // 处理服务器推送消息
                HandlePush_Ex(phobosPacket);
            }
        }

        //发送接口
        private void Call_Ex(string metas,CitrusNet.NetCommand args, CitrusNet.Phobos.RpcCallback<CitrusNet.NetCommand> callback)
        {
            Util.LogWarning($"--< Send     Message >--  msg: {args}      route:  {GetValue("route", args)}");
            this.Call(metas, args, callback);
        }

        /// <summary>
        /// 发送SOCKET消息，
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="action"></param>
        public void SendMessage(CitrusNet.NetCommand cmd, Action<CitrusNet.NetCommand> action = null)
        {
            this.Call_Ex("route,session", cmd, (header, args) =>
            {
                if (header.Errno != 0)
                {
                    Util.LogWarning($"errno: {header.Errno}, error: {header.Error}, reply: {args}");
                    return;
                }
                if (action != null) action(args);
            });
        }
        /// <summary>
        /// 推送SOCKET消息
        /// </summary>
        public void PushMessage(CitrusNet.NetCommand cmd)
        {
            this.Send(cmd);
        }

        /// <summary>
        /// 请求连接
        /// </summary>
        public void SendConnect(string _server, int _port, XLua.LuaFunction function, int timeoutMS = 3000)
        {
            Util.Log($"{_server}:{_port} Connecting.... {DateTime.Now.ToString()}");
            this.Connect(_server, _port, (ex) => {
                if (function != null) function.Action(ex);
            if (ex != null)
            {
                Util.LogError($"{_server}:{_port} connecting error: {ex.Message}");
                return;
            }

            Util.Log($"{_server}:{_port} connecting: {ex == null} {ex}");


                //test
            // 设置元数据
//            SetValue("player", "10086");
//            SetValue("session", "1111111");
            //NetworkManager.AddEvent(Protocal.Connect, new ByteNetCommand());

//                 var login = new logic.proto.CS_Player_Login { token = "11010", };
//                 this.SendMessage(login, (cmd) =>
//                 {
//                     //var sc_login = cmd as logic.proto.SC_Player_Login;
//                     //Util.Log($"login reslut --> {sc_login} id:{sc_login.id} ");
//                     NetworkManager.AddEvent(Protocal.Message, cmd as ByteNetCommand);
//                 });

            }, timeoutMS);
        }

        /// <summary>
        /// 重连
        /// </summary>
        public void ReConnect()
        {

        }
        /// <summary>
        /// 注册协议
        /// </summary>
        /// <param name="protos"></param>
        public void RegisterProtos(Dictionary<uint, Type> protos)
        {
            if (protos == null) return;

            foreach (var item in protos)
            {
                Registry.Register(item.Key, item.Value);
            }
            protos.Clear();
        }
    }
}

