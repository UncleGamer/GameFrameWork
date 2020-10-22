using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 网络协议注册类，后面需要自动生成，目前手动填写
/// </summary>
public static class NetWorkRegister 
{
    public static Dictionary<uint, Type> protos = new Dictionary<uint, Type>
    {
         {(uint)logic.proto.EMsgIds.ECS_Player_Tick, typeof(logic.proto.CS_Player_Tick)},
         //{(uint)logic.proto.EMsgIds.ESC_Player_Tick, typeof(logic.proto.SC_Player_Tick)},
         {(uint)logic.proto.EMsgIds.ECS_Player_Login, typeof(logic.proto.CS_Player_Login)},
         //{(uint)logic.proto.EMsgIds.ESC_Player_Login, typeof(logic.proto.SC_Player_Login)},
         {112, typeof(PhobosRpc.PlayerLogin)},
    };
}
