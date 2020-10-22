--[[
-- added by wsh @ 2017-12-08
-- Lua侧UILayer
--]]

local UILayer = BaseClass("UILayer")
local function __init(self, holder, var_arg)
	-- 持有者
	self.holder = holder
	-- transform对应的gameObject
	self.gameObject = nil
	-- 可变类型参数，用于重载
	self.__var_arg = var_arg
	-- 名字
	self.__name = nil
end
-- 创建
local function OnCreate(self, layer)
	self.gameObject = UIUtil.Find(self.holder.gameObject, self.__var_arg)
	-- Unity侧原生组件
	self.unity_canvas = nil
	self.unity_canvas_scaler = nil
	self.unity_graphic_raycaster = nil
	
	-- ui layer
	self.gameObject.layer = 5

	--名字
	self.__name = self.gameObject.name
	
	-- canvas
	self.unity_canvas = UIUtil.GetComponent(self.gameObject, typeof(CS.UnityEngine.Canvas))
	if IsNull(self.unity_canvas) then
		self.unity_canvas = self.gameObject:AddComponent(typeof(CS.UnityEngine.Canvas))
		self.transform = self.unity_canvas.transform
		self.gameObject = self.unity_canvas.gameObject
	end
	self.unity_canvas.renderMode = CS.UnityEngine.RenderMode.ScreenSpaceCamera
	self.unity_canvas.worldCamera = UIManager:GetInstance().UICamera
	self.unity_canvas.planeDistance = layer.PlaneDistance
	self.unity_canvas.sortingLayerName = SortingLayerNames.UI
	self.unity_canvas.sortingOrder = layer.OrderInLayer
	
	-- scaler
	self.unity_canvas_scaler = UIUtil.TryGetComponent(self.gameObject, typeof(CS.UnityEngine.UI.CanvasScaler))
	self.unity_canvas_scaler.uiScaleMode = CS.UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize
	self.unity_canvas_scaler.screenMatchMode = CS.UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight
	self.unity_canvas_scaler.referenceResolution = UIManager:GetInstance().Resolution
	
	-- raycaster
	self.unity_graphic_raycaster = UIUtil.TryGetComponent(self.gameObject, typeof(CS.UnityEngine.UI.GraphicRaycaster))
	
	-- window order
	self.top_window_order = layer.OrderInLayer
	self.min_window_order = layer.OrderInLayer
end

-- pop window order
local function PopWindowOrder(self)
	local cur = self.top_window_order
	self.top_window_order = self.top_window_order + UIManager:GetInstance().MaxOrderPerWindow
	return cur
end

-- push window order
local function PushWindowOrder(self)
	assert(self.top_window_order > self.min_window_order)
	self.top_window_order = self.top_window_order - UIManager:GetInstance().MaxOrderPerWindow
end

-- 销毁
local function OnDestroy(self)
	self.unity_canvas = nil
	self.unity_canvas_scaler = nil
	self.unity_graphic_raycaster = nil
end

-- 获取名字
local function GetName(self)
	return self.__name
end

UILayer.__init = __init
UILayer.OnCreate = OnCreate
UILayer.PopWindowOrder = PopWindowOrder
UILayer.PushWindowOrder = PushWindowOrder
UILayer.OnDestroy = OnDestroy
UILayer.GetName = GetName

return UILayer