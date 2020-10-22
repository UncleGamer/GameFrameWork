--[[
-- added by wsh @ 2017-11-28
-- 单元测试
-- 修改或者添加核心、公共脚本以后最好写并跑一遍单元测试，确保没问题，降低错误和调试难度
--]]

local LoggerTest = require "UnitTest.LoggerTest"
local NetWorkTest = require "UnitTest.NetWorkTest"

local function LoopRunTimes(unitTests, times)
	for i = 1,times do
		--print("-------------------LoopUnitTest["..i.."]-------------------")
		for _,test in pairs(unitTests) do
			test.Run()
		end
		coroutine.waitforframes(1)
		--collectgarbage()
		--print("use mem : "..collectgarbage("count").."KB")
	end
end

local function Run()
	LoggerTest.Run()
	NetWorkTest.Run()
end

return {
	Run = Run
}