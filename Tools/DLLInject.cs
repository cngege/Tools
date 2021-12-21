using System;
using System.Collections.Generic;
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
            Fail_CreateRemoteThread
        }

        public class DLLInject
        {
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
                IntPtr applyptr = Address.Address.VirtualAllocEx(hProcess, IntPtr.Zero, dllpath.Length+1, Address.Address.MEM_COMMIT | Address.Address.MEM_RESERVE, Address.Address.PAGE_READWRITE);
                if (applyptr == IntPtr.Zero)
                {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_ApplyMemory;
                }
                byte[] dllpath_byte = Encoding.Default.GetBytes(dllpath);
                Address.Address.WriteValue_bytes(applyptr, dllpath_byte, hProcess);
                if (Address.Address.CreateRemoteThread(hProcess, 0, 0, Address.Address.GetProcAddress(Address.Address.GetModuleHandleA("Kernel32"), "LoadLibraryA"), applyptr, 0, 0) == 0)
                {
                    Address.Address.CloseHandle(hProcess);
                    return InjectState.Fail_CreateRemoteThread;
                }
                Address.Address.VirtualFreeEx(hProcess, applyptr, 0, Address.Address.MEM_RELEASE);
                Address.Address.CloseHandle(hProcess);
                return InjectState.Success;
            }
        }
    }
}
