using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    //窗口操作
    namespace Formoperate
    {
        public class WinForms
        {
            /// <summary>
            /// 释放鼠标资源 否则SeedMessage可能会失败
            /// </summary>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

            /// <summary>
            /// 移动窗口，代码放在Down事件下
            /// </summary>
            /// <param name="handle">窗口句柄[(IntPtr)this.handle]</param>
            /// <returns>消息是否发送成功</returns>
            public static bool MoveFrom(IntPtr handle)
            {
                ReleaseCapture();
                return SendMessage(handle, 0XA1, 2, 0);
            }


            public static void SetThreadtoUI()
            {
                //CheckForIllegalCrossThreadCalls = false;
            }

            /// <summary>
            /// 发送窗口消息修改窗口的状态 最大化 最小化 关闭 还原
            /// </summary>
            /// <param name="form">窗口句柄</param>
            /// <param name="size">0：关闭，1：最小化，2：还原，3：最大化</param>
            public static void FormMessage(IntPtr form, int size)
            {
                ReleaseCapture();
                switch (size)
                {
                    case 0: SendMessage(form, 0x112, 0xf060, 0); break;//关闭
                    case 1: SendMessage(form, 0x112, 0xf020, 0); break;//最大化
                    case 2: SendMessage(form, 0x112, 0xf120, 0); break;//还原
                    case 3: SendMessage(form, 0x112, 0xf030, 0); break;//最小化
                }
            }


        }
    }
}
