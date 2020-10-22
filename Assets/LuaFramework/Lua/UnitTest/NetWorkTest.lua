local MsgIDMap = require "Net.Config.MsgIDMap"
local MsgIDDefine = require "Net.Config.MsgIDDefine"

local function TeseSendMessage()
	local func = function (self)
		local msg_obj = { token = "11010", }
		HallConnector:GetInstance():SendMessage(MsgIDMap[MsgIDDefine.LOGIN_REQ_LOGIN], msg_obj, nil, function (data)
			print('data:', data.id)
		end)
	end
	HallConnector:GetInstance():Connect("192.168.213.115", 20121, nil, func)
end

local function Run()
	TeseSendMessage()
end

return {
	Run = Run
}