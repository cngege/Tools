using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    namespace KeyHook
    {
        /// <summary>
        /// 键盘钩子
        /// </summary>
        public class KeyBoardHook
        {
            public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
            public const int WH_KEYBOARD_LL = 13;
            public int hHook = 0;
            [StructLayout(LayoutKind.Sequential)]
            public class KeyBoardHookStruct
            {
                public int vkCode;
                public int scanCode;
                public int flags;
                public int time;
                public int dwExtraInfo;
            }//函数声明
            [DllImport("user32.dll")]
            public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            //抽掉钩子  
            public static extern bool UnhookWindowsHookEx(int idHook);
            [DllImport("user32.dll")]
            //调用下一个钩子  
            public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

            [DllImport("kernel32.dll")]
            public static extern int GetCurrentThreadId();

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string name);
            [DllImport("User32.dll")]
            public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);

            public void Hook_Start(HookProc KeyBoardHookProcedure)
            {
                // 安装键盘钩子  
                if (hHook == 0)
                {
                    //KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);
                    hHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                              KeyBoardHookProcedure,
                            GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                    //如果设置钩子失败.  
                    if (hHook == 0)
                    {
                        Hook_Clear();
                    }
                }
            }

            //取消钩子事件  
            public void Hook_Clear()
            {
                bool retKeyboard = true;
                if (hHook != 0)
                {
                    retKeyboard = UnhookWindowsHookEx(hHook);
                    hHook = 0;
                }
                //如果去掉钩子失败.  

            }

        }
    
        public class MouseHook
        {
            //模拟鼠标右键按下 
            public static int MOUSEEVENTF_RIGHTDOWN = 0x0008;
            //模拟鼠标右键抬起 
            public static int MOUSEEVENTF_RIGHTUP = 0x0010;

            [System.Runtime.InteropServices.DllImport("user32")]
            public static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        }

    }

}
