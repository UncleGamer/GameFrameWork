using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    [ExecuteInEditMode]
    [AddComponentMenu(ComponentMenuRoot + "Localized Text")]
    public class LocalizedTextBehaviour : GameToolkit.Localization.LocalizedAssetBehaviour
    {
        public UnityEngine.UI.Text text;
        public string key = "";
        protected override bool TryUpdateComponentLocalization(bool isOnValidate)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (text)
                {
                    text.text = "";
                    return true;
                }
            }
            return false;
        }

    }

}
