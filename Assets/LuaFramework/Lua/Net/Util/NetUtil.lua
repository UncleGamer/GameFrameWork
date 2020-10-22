--[[
-- added by wsh @ 2017-01-10
-- 网络模块工具类
--]]

local NetUtil = {}
require "Net.Config.proto"
local unpack = unpack or table.unpack
local MsgIDMap = require "Net.Config.MsgIDMap"
local ReceiveSinglePackage = require "Net.Config.ReceiveSinglePackage"
local ReceiveMsgDefine = require "Net.Config.ReceiveMsgDefine"


local function SerializeMessage(msg_id, msg_obj)
	assert(msg_obj ~= nil and type(msg_obj) == "table", "SerializeMessage supported 'table'! ")
	local pid = tonumber(msg_id)
	local msgname = MsgIDMap[pid]
	local buf = assert(pb.encode(msgname, msg_obj))
	
	local output = CS.LuaFramework.ByteNetCommand()
	output.pid = pid
	output.msgname = msgname
	output.buf = buf
	return output
end

local function DeserializeMessage(data)
	assert(data ~= nil and type(data) == "userdata", "DeserializeMessage supported 'userdata'! ")

	local pid = data.pid
	local msgname = MsgIDMap[pid]
	local recvData = assert(pb.decode(msgname, data.buf))
	return recvData
end

NetUtil.SerializeMessage = SerializeMessage
NetUtil.DeserializeMessage = DeserializeMessage

return ConstClass("NetUtil", NetUtil)