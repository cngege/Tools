using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Tools.Address;

namespace Tools
{
    //数据处理
    namespace DLLInject
    {
        public enum InjectState
        {
            /// <summary>
            /// 未知原因的失败
            /// </summary>
            Fail,
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 打开进程失败
            /// </summary>
            Fail_OpenProcess,
            /// <summary>
            /// 不能从该PID中找到相关进程
            /// </summary>
            ProcessNoFound,
            /// <summary>
            /// 申请一段内存失败
            /// </summary>
            Fail_ApplyMemory,
            /// <summary>
            /// 创建远程线程失败
            /// </summary>
            Fail_CreateRemoteThread,
            /// <summary>
            /// 没有能获取线程退出返回码
            /// </summary>
            Fail_NoFoundExitCode,
            /// <summary>
            /// 退出码为0
            /// </summary>
            Fail_ExitCodeIsZero
        }

        public class DLLInject
        {
            [DllImport("kernel32.dll")]
            private static extern int WaitForSingleObject(IntPtr hwnd, int dwMilliseconds);
            [DllImport("kernel32.dll")]
            private static extern bool GetExitCodeThread(IntPtr hwnd, out IntPtr lpExitCode);

            // 有一个注入的方法
            /// <summary>
            /// 注入dll
            /// </summary>
            /// <param name="pid"></param>
            /// <param name="dllpath">要注入的dll的绝对路径</param>
            /// <returns>返回一个注入结果的状态信息</returns>
            public static InjectState Inject(int pid, string dllpath)
            {
                IntPtr hProcess = Address.Address.OpenProcess(0x1F0FFF, false, pid);
                if (hProcess == IntPtr.Zero)
                {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_OpenProcess;
                }
                
                bool isUnicode = CheckExistUnicode(dllpath);

                byte[] dllpath_byte = isUnicode ? Encoding.Unicode.GetBytes(dllpath) : Encoding.UTF8.GetBytes(dllpath);
                IntPtr applyptr = Address.Address.VirtualAllocEx(hProcess, IntPtr.Zero, dllpath_byte.Length + 1, Address.Address.MEM_COMMIT | Address.Address.MEM_RESERVE, Address.Address.PAGE_READWRITE);
                if (applyptr == IntPtr.Zero) {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_ApplyMemory;
                }
                Address.Address.WriteValue_bytes(applyptr, dllpath_byte, hProcess);
                IntPtr Thread_LL = Address.Address.CreateRemoteThread(hProcess, 0, 0, Address.Address.GetProcAddress(Address.Address.GetModuleHandleA("Kernel32"), isUnicode ? "LoadLibraryW" : "LoadLibraryA"), applyptr, 0, out int threadid);
                if (Thread_LL == IntPtr.Zero) {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_CreateRemoteThread;
                }
                _ = WaitForSingleObject(Thread_LL, int.MaxValue);
                if (!GetExitCodeThread(Thread_LL, out IntPtr ExitCode)) {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_NoFoundExitCode;
                }
                if (ExitCode == IntPtr.Zero) {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_ExitCodeIsZero;
                }

                Address.Address.CloseHandle(hProcess);
                return InjectState.Success;
            }


            private static bool CheckExistUnicode(string strInput) {
                int i = strInput.Length;
                if (i == 0)
                    return false;
                int j = System.Text.Encoding.Default.GetBytes(strInput).Length;
                if (i != j)
                    return true;
                else
                    return false;
            }
        }
    }
}
