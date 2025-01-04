using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

            [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
            public static extern IntPtr CreateRoundRectRgn(
                int nLeftRect, // x-coordinate of upper-left corner
                int nTopRect, // y-coordinate of upper-left corner
                int nRightRect, // x-coordinate of lower-right corner
                int nBottomRect, // y-coordinate of lower-right corner
                int nWidthEllipse, // height of ellipse
                int nHeightEllipse // width of ellipse
            );

            /// <summary>
            /// 设置窗口为圆角
            /// </summary>
            /// <param name="handle"></param>
            /// <param name="widthEllipse"></param>
            /// <param name="heightEllipse"></param>
            public static void SetFormRound(Form handle, int widthEllipse, int heightEllipse) {
                handle.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, handle.Width, handle.Height, widthEllipse, heightEllipse));
            }

            /// <summary>
            /// 移动窗口，代码放在Down事件下
            /// </summary>
            /// <param name="handle">窗口句柄[(IntPtr)this.handle]</param>
            /// <returns>消息是否发送成功</returns>
            public static bool MoveForm(IntPtr handle)
            {
                ReleaseCapture();
                return SendMessage(handle, 0XA1, 2, 0);
            }

            /// <summary>
            /// 关闭UI对线程的错误捕捉[还未开发]
            /// </summary>
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
