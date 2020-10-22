using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    public class SceneManager : Manager
    {
        //开始回调
        public Action startLoadActon = null;
        //结束回调
        public Action endLoadActon = null;
        //进度回调
        public XLua.LuaFunction progressLoadActon = null;

        //空场景，场景切换时，在切换至空场景
        public string emptyScene = "LoadingScene";


        private void StartSwitchSceneMessage()
        {
            if (startLoadActon != null) startLoadActon();
        }

        private void EndSwitchSceneMessage()
        {
            if (endLoadActon != null) endLoadActon();
        }

        private void ProgressSwitchSceneMessage(float progress)
        {
            if (progressLoadActon != null) progressLoadActon.Action<float>(progress);
        }

        private IEnumerator LoadScene(string scenename, bool bAsync)
        {
            this.StartSwitchSceneMessage();

            this.ProgressSwitchSceneMessage(0); yield return new WaitForEndOfFrame();

            //先加载空场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(emptyScene);

            this.ProgressSwitchSceneMessage(50); yield return new WaitForEndOfFrame();

            //切换到scenename
            if (bAsync)
            {
                AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenename);
                while (!operation.isDone)
                {
                    this.ProgressSwitchSceneMessage(50 + operation.progress % 50); yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(scenename);
            }

            this.ProgressSwitchSceneMessage(100); yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(0.5f);
            this.EndSwitchSceneMessage();

        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="scenename"></param>
        public void SwitchScene(string scenename)
        {
            StartCoroutine(LoadScene(scenename, false));
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="scenename"></param>
        public void AsyncSwitchScene(string scenename)
        {
            StartCoroutine(LoadScene(scenename, true));
        }

        private void OnDestroy()
        {
            startLoadActon = null;
            endLoadActon = null;
            progressLoadActon = null;
        }
    }
}
