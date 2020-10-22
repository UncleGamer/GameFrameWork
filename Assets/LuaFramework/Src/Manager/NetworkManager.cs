using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LuaFramework {
	public class NetworkManager : Manager {

		private static readonly object m_lockObject = new object();
        private static Queue<KeyValuePair<int, ByteNetCommand>> mEvents = new Queue<KeyValuePair<int, ByteNetCommand>>();

        private List<CitrusNetWork> netWorkList = new List<CitrusNetWork>();
        

		void Awake() {
			Init();
		}

		void Init() {
        }

		public void OnInit() {
            //启动Lua网络库
            //CallMethod("Start");

            //CreateNetWork(NetWorkRegister.protos);
            //mNetwork.SendConnect("192.168.213.115", 20121);
            
        }

		public void Unload() {
			//CallMethod("Unload");
		}

        /// <summary>
        /// 创建一个网络
        /// </summary>
        public CitrusNetWork CreateNetWork(Dictionary<uint, Type> protos = null)
        {
            CitrusNetWork mNetwork = new CitrusNetWork();
            mNetwork.OnConnHeartbeat = () => {
                //心跳包
                mNetwork.SendMessage(new logic.proto.CS_Player_Tick { msg = "test" },(cmd)=> {
                    NetworkManager.AddEvent(Protocal.Message, (ByteNetCommand)cmd);
                });
            };
            mNetwork.RegisterProtos(protos);
            netWorkList.Add(mNetwork);

            return mNetwork;
        }
        /// <summary>
        /// 创建一个网络（lua侧使用）
        /// </summary>
        /// <param name="HeartbeatInterval">心跳时间</param>
        /// <param name="DetectAliveTime">超时检测</param>
        /// <param name="OnConnHeartbeat">心跳回调</param>
        /// <param name="OnConnLoss">断线回调</param>
        public CitrusNetWork CreateNetWork(float HeartbeatInterval, float DetectAliveTime, Action OnConnHeartbeat, Action OnConnLoss)
        {
            var network = new CitrusNetWork();
            network.OnConnHeartbeat = OnConnHeartbeat;
            network.OnConnLoss = OnConnLoss;
            network.HeartbeatInterval = HeartbeatInterval;
            network.DetectAliveTime = DetectAliveTime;
            netWorkList.Add(network);

            return network;
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        private object[] CallMethod(string func, params object[] args) {
			return Util.CallMethod("Network", func, args);
		}

		///------------------------------------------------------------------------------------
		public static void AddEvent(int _event, ByteNetCommand data) {
			lock (m_lockObject) {
				mEvents.Enqueue(new KeyValuePair<int, ByteNetCommand>(_event, data));
			}
		}

		/// <summary>
		/// 交给Command，这里不想关心发给谁。
		/// </summary>
		void Update() {

            foreach (var net in netWorkList)
            {
                net.Update(Time.deltaTime);
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            foreach (var net in netWorkList)
            {
                net.Close();
            }
            Util.Log("~NetworkManager was destroy");
        }


        /// <summary>
        /// 主动断开连接
        /// </summary>
        public void Logout()
        {
            foreach (var net in netWorkList)
            {
                net.Close();
            }
            Util.Log($"Logout.... {DateTime.Now.ToString()}");
        }


    }
}