using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuaFramework
{
    public class Utils
    {
        static public GameObject AddChild(GameObject parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }
        // 查找子窗口
        public static GameObject FindChild(GameObject parent, string name)
        {
            Transform tmp;
            for (int i = 0; i < parent.transform.childCount; ++i)
            {
                if ((tmp = parent.transform.GetChild(i)).name == name)
                    return tmp.gameObject;

                //if ((obj = FindChild(tmp.gameObject, name)) != null) 
                //    return obj;
            }
            Util.Log("name:" + name + " not find! ");
            return null;
        }

        // 查找子窗口,通过分隔符'/'来确定父子窗口
        public static GameObject Find(GameObject parent, string name)
        {
            string[] childs = name.Split('/');
            Transform p = parent.transform;
            foreach (string child in childs)
            {
                p = p.transform.Find(child);
                if (p == null)
                {
                    Util.Log("name:" + name + " not find! ");
                    return null;
                }
            }

            return p.gameObject;
        }
        // 查找子窗口,通过分隔符'/'来确定父子窗口
        public static T Find<T>(GameObject parent, string name) where T : Component
        {
            string[] childs = name.Split('/');
            Transform p = parent.transform;
            foreach (string child in childs)
            {
                p = p.transform.Find(child);
                if (p == null)
                {
                    Util.LogError(name + " name:" + typeof(T).Name + " not find! ");
                    return null;
                }
            }

            return p.gameObject.GetComponent<T>();
        }

        public static Text FindText(GameObject parent, string name) { return Find<Text>(parent, name); }
        public static Image FindImage(GameObject parent, string name) { return Find<Image>(parent, name); }
        public static ScrollRect FindScrollRect(GameObject parent, string name) { return Find<ScrollRect>(parent, name); }
        public static Grid FindGrid(GameObject parent, string name) { return Find<Grid>(parent, name); }
        public static InputField FindInputField(GameObject parent, string name) { return Find<InputField>(parent, name); }
        public static Slider FindSlider(GameObject parent, string name) { return Find<Slider>(parent, name); }
        public static Button FindButton(GameObject parent, string name) { return Find<Button>(parent, name); }
    }
}
