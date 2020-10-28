using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XLua;
using System;

namespace LuaFramework {

    public class PanelManager : Manager {
		private Transform parent;

		Transform Parent {
			get {
				if (parent == null) {
					GameObject go = GameObject.FindWithTag("GuiCamera");
					if (go != null) parent = go.transform;
				}
				return parent;
			}
		}
        //面板资源路径
        private Dictionary<PanelId, string> panelPaths;
        //面板对象
        private Dictionary<PanelId, Type> panelTypes;
        //已经加载
        private Dictionary<PanelId, GameObject> panelLoads;
        /// <summary>
        /// 暴露给lua的update
        /// </summary>
        public LuaFunction updateFunction = null;

        private void Awake()
        {
            this.Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            panelPaths = new Dictionary<PanelId, string>();
            panelTypes = new Dictionary<PanelId, Type>();

            //初始化，找到所有的面板
            var typelist = Util.GetAllTypes(typeof(LuaFrameworkPanelAttribute));
            for (int i = 0; i < typelist.Count; i++)
            {
                Type panel_type = typelist[i];
                LuaFrameworkPanelAttribute att = (LuaFrameworkPanelAttribute)Attribute.GetCustomAttribute(panel_type, typeof(LuaFrameworkPanelAttribute));
                PanelId panel_id = att.panelId;
                string panel_path = att.resPath;

                panelPaths.Add(panel_id, panel_path);
                panelTypes.Add(panel_id, panel_type);
            }
        }


        /// <summary>
        /// 创建面板，请求资源管理器
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, Action<GameObject> func = null) {
			string assetName = name + "Panel";
			string abName = name.ToLower() + AppConst.ExtName;
			if (Parent.Find(name) != null) return;

			#if ASYNC_MODE
			ResManager.LoadPrefab(abName, assetName, delegate(UnityEngine.Object[] objs) {
			if (objs.Length == 0) return;
			GameObject prefab = objs[0] as GameObject;
			if (prefab == null) return;

			GameObject go = Instantiate(prefab) as GameObject;
			go.name = assetName;
			go.layer = LayerMask.NameToLayer("Default");
			go.transform.SetParent(Parent);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.AddComponent<LuaBehaviour>();

			if (func != null) func(go);
			Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
			});
			#else
			GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
			if (prefab == null) return;

			GameObject go = Instantiate(prefab) as GameObject;
			go.name = assetName;
			go.layer = LayerMask.NameToLayer("Default");
			go.transform.SetParent(Parent);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.AddComponent<LuaBehaviour>();

			if (func != null) func(go);
			Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
			#endif
		}

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panel"></param>
        public void OpenPanel(PanelId panel)
        {
            //path
            string panel_path = "";
            if(!panelPaths.TryGetValue(panel, out panel_path))
            {
                return;
            }
            //type
            Type panel_type = null;
            if (!panelTypes.TryGetValue(panel, out panel_type))
            {
                return;
            }
            //加载, 只允许一个面板实例存在
            GameObject panelObject = null;
            if (!panelLoads.TryGetValue(panel, out panelObject))
            {
                CreatePanel(panel_path, (go) =>
                {
                    if (go != null)
                    {
                        go.AddComponent(panel_type);
                        go.SetActive(true);
                    }
                });
            }
            else
            {
                panelObject.SetActive(false);
                panelObject.SetActive(true);
            }

        }
        /// <summary>
        /// 关闭面板,会直接销毁
        /// </summary>
        /// <param name="name"></param>
        public void ClosePanel(PanelId panel) {

            GameObject panelObject = null;
            if (panelLoads.TryGetValue(panel, out panelObject))
            {
                panelLoads.Remove(panel);
                Destroy(panelObject);
            }
        }


        /// <summary>
        /// 获取UI对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public void LoadUIPanel(string path, System.Action<GameObject> action)
        {
            UnityEngine.Object @object = ResManager.LoadGameObject(path);
            GameObject uiObject = GameObject.Instantiate(@object) as GameObject;
            if (action != null)
                action(uiObject);
        }


        private void Update()
        {
            if (updateFunction != null)
                updateFunction.Action<float>(Time.deltaTime);
        }
        private void OnDestroy()
        {
            updateFunction = null;
        }


    }
}