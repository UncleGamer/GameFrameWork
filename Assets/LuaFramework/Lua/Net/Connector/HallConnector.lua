--[[
-- added by wsh @ 2017-01-09
-- 大厅网络连接器
--]]

local HallConnector = BaseClass("HallConnector", Singleton)
local NetUtil = require "Net.Util.NetUtil"

local function __init(self)
	self.hallSocket = nil
	self._ip = nil
	self._port = nil
end

--请求连接
local function Connect(self, host_ip, host_port, sInstance, func)
	if not self.hallSocket then
		--心跳
		local OnConnHeartbeat = function ()
			HallConnector:GetInstance():ConnectHeartbeat()
		end
		--断线
		local OnConnLoss = function ()
			HallConnector:GetInstance():ConnectError("OnConnect Loss")   
		end
		self.hallSocket = LuaHelper.GetNetManager():CreateNetWork(3, 10, OnConnHeartbeat, OnConnLoss)
	end

	--连接
	local OnConnConnect = function (ex)
		print('OnConnConnect,',ex)

		if ex then
			HallConnector:GetInstance():ConnectError(ex)
		else
			
			HallConnector:GetInstance():ConnectCallback()

			if func then func(sInstance) end
		end
	end

	self._ip = tostring(host_ip)
	self._port = tonumber(host_port)
	self.hallSocket:SendConnect(self._ip, self._port, OnConnConnect)
	Logger.Log("Connect to "..host_ip..", port : "..host_port)
	return self.hallSocket
end

--连接的回调  内部调用
local function ConnectCallback(self)
	print('ConnectCallback successed')
	--1.设置元数据
	self.hallSocket:SetValue("player", "10086")
	self.hallSocket:SetValue("session", "1111111")
end

--连接的异常回调  内部调用
local function ConnectError(self, ex)
	local msg = (ex ~= nil and type(ex) == 'string') and ex or ex.Message
	--1. 弹窗提示
	print('弹窗提示 OnConnLoss: ', msg)
end

--心跳检测的回调  内部调用
local function ConnectHeartbeat(self)
	print('OnConnHeartbeat')
end

local function SendMessage(self, msg_id, msg_obj, sInstance, func)
	if self.hallSocket then
		local seCmd = NetUtil.SerializeMessage(msg_id, msg_obj)
		local callback = function (args)
			local deseCmd = NetUtil.DeserializeMessage(args)
			assert(deseCmd ~= nil, "SendMessage DeserializeMessage error!")
			if func then func(sInstance, deseCmd) end
		end
		self.hallSocket:SendMessage(seCmd, callback)
	end
end

local function Disconnect(self)
	if self.hallSocket then
		self.hallSocket:Close()
	end
end

local function Dispose(self)
	if self.hallSocket then
		self.hallSocket:Close()
	end
	self.hallSocket = nil
end

HallConnector.__init = __init
HallConnector.Connect = Connect
HallConnector.ConnectCallback = ConnectCallback
HallConnector.ConnectError = ConnectError
HallConnector.ConnectHeartbeat = ConnectHeartbeat
HallConnector.SendMessage = SendMessage
HallConnector.Dispose = Dispose

return HallConnector
