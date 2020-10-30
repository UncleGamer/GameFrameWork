using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaFramework
{
    /// <summary>
    /// 面板id
    /// </summary>
    public enum PanelId: uint
    {
        Panel_NULL = 0,
        Panel_Test = 1,//测试面板
    }

    [LuaFrameworkPanel(PanelId.Panel_Test, "UI/Prefabs/View/UILogin.prefab")]
    public class TestPanel : View
    {

    }

}
