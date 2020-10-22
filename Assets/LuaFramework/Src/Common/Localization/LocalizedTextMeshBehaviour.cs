using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LuaFramework
{
    [ExecuteInEditMode]
    [AddComponentMenu(ComponentMenuRoot + "Localized Text Mesh")]
    public class LocalizedTextMeshBehaviour : GameToolkit.Localization.LocalizedAssetBehaviour
    {
        public TMPro.TextMeshProUGUI textMeshPro;
        public string key;
        protected override bool TryUpdateComponentLocalization(bool isOnValidate)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (textMeshPro != null && !string.IsNullOrEmpty(key))
                {
                    string path = key + "_" + GameToolkit.Localization.Localization.Instance.CurrentLanguage.Code + ".asset";
                    Debug.Log("LocalizedTextMeshBehaviour path:" + path);
                    //textMeshPro.font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
                    return true;
                }
            }
            return false;
        }
    }

}