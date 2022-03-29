using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace 控制台游戏
{
    internal abstract class XInput
    {
        // 关于 0x8000 (http://bingtears.iteye.com/blog/663149)
        internal const Int32 KEY_STATE = 0x8000;

        /// <summary>
        /// 判断被调用时指定虚拟键的状态
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        protected static extern Int16 GetAsyncKeyState(System.Int32 vKey);
    }
}
