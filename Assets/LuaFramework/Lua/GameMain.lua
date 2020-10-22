-- 全局模块
require "Global.Global"
	
-- 定义为全局模块，整个lua程序的入口类
GameMain = {};

-- 全局初始化
local function Initilize()
	--local loadingAssetbundlePath = "UI/Prefabs/View/UILoading.prefab"
	--ResourcesManager:GetInstance():CoLoadAssetBundleAsync(loadingAssetbundlePath)
end

-- 进入游戏
local function EnterGame()
	SceneManager:GetInstance():SwitchScene(SceneConfig.LoginScene)
	
end

--主入口函数。从这里开始lua逻辑
local function Start()
	print("GameMain start...")
	
	-- 模块启动
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()
	LogicUpdater:GetInstance():Startup()
	UIManager:GetInstance():Startup()
	
	if Config.UnitTest then
		-- 单元测试
		local UnitTest = require "UnitTest.UnitTestMain"
		UnitTest.Run()
		return
	end
	
	local co = coroutine.create(function()
		Initilize()
		EnterGame()
	end)
	coroutine.resume(co)
end

-- 场景切换通知
local function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

local function OnDestroy()

end

local function OnApplicationQuit()
	-- 模块注销
	UIManager:GetInstance():Dispose()
	UpdateManager:GetInstance():Dispose()
	TimerManager:GetInstance():Dispose()
	LogicUpdater:GetInstance():Dispose()
end

-- GameMain公共接口，其它的一律为私有接口，只能在本模块访问
GameMain.Start = Start
GameMain.OnLevelWasLoaded = OnLevelWasLoaded
GameMain.OnDestroy = OnDestroy
GameMain.OnApplicationQuit = OnApplicationQuit

return GameMain