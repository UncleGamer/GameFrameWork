using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    [ExecuteInEditMode]
    [AddComponentMenu(ComponentMenuRoot + "Localized Font")]
    public class LocalizedFontBehaviour : GameToolkit.Localization.LocalizedAssetBehaviour
    {
        public UnityEngine.UI.Text font;
        public string key;
        protected override bool TryUpdateComponentLocalization(bool isOnValidate)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (font != null && !string.IsNullOrEmpty(key))
                {
                    string path = key + "_" + GameToolkit.Localization.Localization.Instance.CurrentLanguage.Code + ".ttf";
                    Debug.Log("LocalizedFontBehaviour path:" + path);
                    //font.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>(path);
                    return true;
                }
            }
            return false;
        }
    }


}
