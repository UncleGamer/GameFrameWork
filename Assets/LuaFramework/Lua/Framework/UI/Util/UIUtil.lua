--[[
-- added by wsh @ 2017-12-03
-- UI工具类
--]]

--[[


-- 获取直属画布
local function GetCanvas(ui_component)
	-- 初始化直属画布
	local canvas = nil
	if ui_component._class_type == UILayer then
		canvas = ui_component
	else
		local now_holder = ui_component.holder
		while now_holder ~= nil do	
			local var = ui_component:GetComponents(UICanvas)
			if table.count(var) > 0 then
				assert(table.count(var) == 1)
				canvas = var[1]
				break
			end
			now_holder = now_holder.holder
		end
	end
	assert(canvas ~= nil)
	return canvas
end
]]

local Utils = CS.LuaFramework.Utils

local function GetComponent(obj, ctype)
	assert(obj ~= nil)
	assert(ctype ~= nil)
	
	local targetTrans = obj
	if targetTrans == nil then
		return nil
	end
	return targetTrans:GetComponent(ctype)
end

local function TryGetComponent(obj, ctype)
	assert(obj ~= nil)
	assert(ctype ~= nil)
	
	local targetTrans = obj
	if targetTrans == nil then
		return nil
	end
	local cmp = targetTrans:GetComponent(ctype)
	if not IsNull(cmp) then
		return cmp
	end
	return targetTrans:AddComponent(ctype)
end

local function GetRectTransform(obj)
	return GetComponent(obj, typeof(CS.UnityEngine.RectTransform))
end

local function GetChild(obj, index)
	return obj:GetChild(index)
end

local function AddChild(parent, prefab)
	return Utils.AddChild(parent, prefab)
end

local function FindChild(obj, path)
	return Utils.FindChild(obj, path)
end

local function Find(obj, path)
	return Utils.Find(obj, path)
end

local function FindButton(obj, path)
	return Utils.FindButton(obj, path)
end

local function FindText(obj, path)
	return Utils.FindText(obj, path)
end

local function FindImage(obj, path)
	return Utils.FindText(obj, path)
end

local function FindGrid(obj, path)
	return Utils.FindGrid(obj, path)
end

local function FindInputField(obj, path)
	return Utils.FindInputField(obj, path)
end

local function FindSlider(obj, path)
	return Utils.FindSlider(obj, path)
end

local function FindScrollRect(obj, path)
	return Utils.FindScrollRect(obj, path)
end

local function AddButtonOnClick(aUIInstance, aButton, aFunc)
	if not IsNull(aButton) then
		aButton.onClick:AddListener(function ()
			aFunc(aUIInstance)
		end)
	end
end

local function ClearButtonOnClick(aButton)
	if not IsNull(aButton) then
		aButton.onClick:RemoveAllListeners()
	end
end

local UIUtil = {}

UIUtil.GetComponent = GetComponent
UIUtil.TryGetComponent = TryGetComponent
UIUtil.GetRectTransform = GetRectTransform
UIUtil.GetChild = GetChild
UIUtil.AddChild = AddChild
UIUtil.FindChild = FindChild
UIUtil.Find = Find
UIUtil.FindTrans = FindTrans
UIUtil.FindText = FindText
UIUtil.FindImage = FindImage
UIUtil.FindButton = FindButton
UIUtil.FindInputField = FindInputField
UIUtil.FindSlider = FindSlider
UIUtil.FindScrollRect = FindScrollRect
UIUtil.AddButtonOnClick = AddButtonOnClick
UIUtil.ClearButtonOnClick = ClearButtonOnClick

return ConstClass("UIUtil", UIUtil)