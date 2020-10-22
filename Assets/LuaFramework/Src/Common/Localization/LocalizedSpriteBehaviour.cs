using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    [ExecuteInEditMode]
    [AddComponentMenu(ComponentMenuRoot + "Localized Sprite")]
    public class LocalizedSpriteBehaviour : GameToolkit.Localization.LocalizedAssetBehaviour
    {
        public UnityEngine.UI.Image image;
        public string key;
        protected override bool TryUpdateComponentLocalization(bool isOnValidate)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (image != null && !string.IsNullOrEmpty(key))
                {
                    string path = key + "_" + GameToolkit.Localization.Localization.Instance.CurrentLanguage.Code + ".png";
                    Debug.Log("LocalizedSpriteBehaviour path:" + path);
                    //image.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>(path);
                    return true;
                }
            }
            return false;
        }
    }

}

