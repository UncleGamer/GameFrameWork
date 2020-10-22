--[[
-- added by wsh @ 2017-11-30
-- UI视图层基类：该界面所有UI刷新操作，只和展示相关的数据放在这，只有操作相关数据放Model去
-- 注意：
-- 1、被动刷新：所有界面刷新通过消息驱动---除了打开界面时的刷新
-- 2、对Model层可读，不可写---调试模式下强制
-- 3、所有写数据、游戏控制操作、网络相关操作全部放Ctrl层
-- 4、Ctrl层不依赖View层，但是依赖Model层
-- 5、任何情况下不要在游戏逻辑代码操作界面刷新---除了打开、关闭界面
--]]

local UIBaseView = BaseClass("UIBaseView")

-- 构造函数：必须把基类需要的所有参数列齐---即使在这里不用，提高代码可读性
-- 子类别再写构造函数，初始化工作放OnCreate
local function __init(self, holder, var_arg, model, ctrl)
	assert(model ~= nil)
	assert(ctrl ~= nil)
	self.ctrl = ctrl
	-- 强制不能直接写Model层
	if Config.Debug then
		self.model = setmetatable({}, {
			__index = model,
			__newindex = function(tb, key, value)
				error("You can't write model derectly!", 2)
			end
		})
	else 
		self.model = model
	end
	-- 持有者
	self.holder = holder
	-- 可变类型参数，用于重载
	self.__var_arg = var_arg
	-- transform对应的gameObject
	self.gameObject = nil
	-- trasnform对应的RectTransform
	self.rectTransform = nil
	-- 窗口画布
	self.canvas = nil
	-- 窗口基础order，窗口内添加的其它canvas设置的order都以它做偏移
	self.base_order = 0
	-- 窗口是否可见
	self.visiable = false
end
-- 析构函数：所有组件的子类不要再写这个函数，释放工作全部放到OnDestroy
local function __delete(self)
	self:OnDestroy()
end
-- 创建：资源加载完毕
local function OnCreate(self)
	-- 初始化其它基本信息
	if type(self.__var_arg) == "string" then
		-- 与持有者的相对路径
		local transform = UIUtil.Find(self.holder.gameObject, self.__var_arg)
		self.gameObject = transform.gameObject
	elseif type(self.__var_arg) == "number" then
		-- 持有者第index个孩子
		local transform = UIUtil.GetChild(self.holder.gameObject, self.__var_arg)
		self.gameObject = transform.gameObject
	elseif type(self.__var_arg) == "userdata" then
		-- Unity侧GameObject
		self.gameObject = self.__var_arg
	else
		error("OnCreate : error params list! "..type(self.__var_arg).." "..tostring(self.__var_arg))
	end
	self.__name = self.gameObject.name
	self.rectTransform = UIUtil.GetComponent(self.gameObject, typeof(CS.UnityEngine.RectTransform))
	self.__var_arg = nil

	-- 窗口画布
	self.canvas = nil--self:AddComponent(UICanvas, "", 0)
	-- 回调管理，使其最长保持和View等同的生命周期
	self.__ui_callback = {}
	-- 初始化RectTransform
	self.rectTransform.offsetMax = Vector2.zero
	self.rectTransform.offsetMin = Vector2.zero
	self.rectTransform.localScale = Vector3.one
	self.rectTransform.localPosition = Vector3.zero
end

-- 激活、反激活
local function SetActive(self, active)
	if self.visiable == active then return end
	self.visiable = active
	if active then
		self.gameObject:SetActive(active)
		self:OnEnable()
	else
		self:OnDisable()
		self.gameObject:SetActive(active)
	end
end

-- 获取激活状态
local function GetActive(self)
	return self.visiable
end

-- 打开：窗口显示
local function OnEnable(self)
	self.base_order = self.holder:PopWindowOrder()
	self:OnAddListener()
end

-- 每帧刷新
local function OnUpdate(self, delta_time)
end

-- 注册消息
local function OnAddListener(self)
end

-- 注销消息
local function OnRemoveListener(self)
end

local function AddCallback(keeper, msg_name, callback)
	assert(callback ~= nil)
	keeper[msg_name] = callback
end

local function GetCallback(keeper, msg_name)
	return keeper[msg_name]
end

local function RemoveCallback(keeper, msg_name, callback)
	assert(callback ~= nil)
	keeper[msg_name] = nil
end

-- 注册UI数据监听事件，别重写
local function AddUIListener(self, msg_name, callback)
	local bindFunc = Bind(self, callback)
	AddCallback(self.__ui_callback, msg_name, bindFunc)
	UIManager:GetInstance():AddListener(msg_name, bindFunc)
end

-- 注销UI数据监听事件，别重写
local function RemoveUIListener(self, msg_name, callback)
	local bindFunc = GetCallback(self.__ui_callback, msg_name)
	RemoveCallback(self.__ui_callback, msg_name, bindFunc)
	UIManager:GetInstance():RemoveListener(msg_name, bindFunc)
end

-- 关闭：窗口隐藏
local function OnDisable(self)
	self:OnRemoveListener()
	self.holder:PushWindowOrder()
end

-- 销毁：窗口销毁
local function OnDestroy(self)
	for k,v in pairs(self.__ui_callback) do
		self:RemoveUIListener(k, v)
	end
	self.model = nil
	self.ctrl = nil
	self.__ui_callback = nil
end

UIBaseView.__init = __init
UIBaseView.__delete = __delete
UIBaseView.OnCreate = OnCreate
UIBaseView.SetActive = SetActive
UIBaseView.GetActive = GetActive
UIBaseView.OnEnable = OnEnable
UIBaseView.OnUpdate = OnUpdate
UIBaseView.OnAddListener = OnAddListener
UIBaseView.OnRemoveListener = OnRemoveListener
UIBaseView.OnDisable = OnDisable
UIBaseView.AddUIListener = AddUIListener
UIBaseView.RemoveUIListener = RemoveUIListener
UIBaseView.OnDestroy = OnDestroy

return UIBaseView