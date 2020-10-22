--[[
-- added by wsh @ 2017-11-30
-- Logger系统,
--]]

local Logger = BaseClass("Logger")

local function Log(msg)
	print(msg)
end

local function LogError(msg)
	error(msg, 2)
end

Logger.Log = Log
Logger.LogError = LogError

return Logger