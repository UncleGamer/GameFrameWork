using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using XLua;
using System;
using System.Collections;

namespace LuaFramework {
	public static class LuaHelper {

        #region 数据类型扩展

		/// <summary>
		/// getType
		/// </summary>
		/// <param name="classname"></param>
		/// <returns></returns>
		public static System.Type GetType(string classname) {
			Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
			System.Type t = null;
			t = assb.GetType(classname); ;
			if (t == null) {
				t = assb.GetType(classname);
			}
			return t;
		}
        // 说明：扩展CreateInstance方法
        public static Array CreateArrayInstance(Type itemType, int itemCount)
        {
            return Array.CreateInstance(itemType, itemCount);
        }

        public static IList CreateListInstance(Type itemType)
        {
            return (IList)Activator.CreateInstance(MakeGenericListType(itemType));
        }

        public static IDictionary CreateDictionaryInstance(Type keyType, Type valueType)
        {
            return (IDictionary)Activator.CreateInstance(MakeGenericDictionaryType(keyType, valueType));
        }

        // 说明：构建List类型
        public static Type MakeGenericListType(Type itemType)
        {
            return typeof(List<>).MakeGenericType(itemType);
        }

        // 说明：构建Dictionary类型
        public static Type MakeGenericDictionaryType(Type keyType, Type valueType)
        {
            return typeof(Dictionary<,>).MakeGenericType(new Type[] { keyType, valueType });
        }

        #endregion

        #region 游戏逻辑扩展

        /// <summary>
        /// 面板管理器
        /// </summary>
        public static PanelManager GetPanelManager() {
			return AppFacade.Instance.GetManager<PanelManager>(ManagerName.Panel);
		}

		/// <summary>
		/// 资源管理器
		/// </summary>
		public static ResourceManager GetResManager() {
			return AppFacade.Instance.GetManager<ResourceManager>(ManagerName.Resource);
		}

		/// <summary>
		/// 网络管理器
		/// </summary>
		public static NetworkManager GetNetManager() {
			return AppFacade.Instance.GetManager<NetworkManager>(ManagerName.Network);
		}

		/// <summary>
		/// 音乐管理器
		/// </summary>
		public static SoundManager GetSoundManager() {
			return AppFacade.Instance.GetManager<SoundManager>(ManagerName.Sound);
		}

        public static SceneManager GetSceneManager() {
            return AppFacade.Instance.GetManager<SceneManager>(ManagerName.Scene);
        }

        #endregion

        /// <summary>
        /// cjson函数回调
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnJsonCallFunc(string data, LuaFunction func) {
			Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
			if (func != null) func.Call(data);
		}
	}
}