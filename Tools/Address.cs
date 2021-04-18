using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    //内存操作
    namespace Address
    {
        public class Address
        {
            //从指定内存中读取字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

            //从指定内存中写入字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "WriteProcessMemory")]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

            //打开一个已存在的进程对象，并返回进程的句柄
            [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            //关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。
            [DllImport("kernel32.dll")]
            private static extern void CloseHandle(IntPtr hObject);

            /// <summary>
            /// 进程名获取Pid
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static int GetPid(String name)
            {
                Process[] arrayProcess = Process.GetProcessesByName(name);
                foreach (Process p in arrayProcess)
                {
                    return p.Id;
                }
                return 0;
            }

            /// <summary>
            /// 读内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="pid">进程Pid</param>
            /// <param name="AddrSize">读取的内存大小</param>
            /// <returns>返回的内存值</returns>
            public static int ReadValue(IntPtr address, int pid, int AddrSize = 4)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, AddrSize, IntPtr.Zero);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 32 位带符号整数。
                    return Marshal.ReadInt32(byteAddress);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 读内存长整数
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="pid">进程Pid</param>
            /// <param name="AddrSize">读取的内存大小</param>
            /// <returns>返回的内存值</returns>
            public static long ReadValue64(IntPtr address, int pid, int AddrSize = 4)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, AddrSize, IntPtr.Zero);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 64 位带符号整数。
                    return Marshal.ReadInt64(byteAddress);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 写内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="value">写入的内存值</param>
            /// <param name="pid">进程pid</param>
            /// <param name="AddrSize">写入的地址大小</param>
            public static void WriteValue(IntPtr address, int value, int pid, int AddrSize = 4)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, address, new int[] { value }, AddrSize, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }

            /// <summary>
            /// 内存搜索特征码
            /// 可能会卡死数分钟，考虑在线程中操作
            /// </summary>
            /// <param name="startAddress">开始查询地址,一般为基址</param>
            /// <param name="pid">程序Pid</param>
            /// <param name="memoryBlock">特征码</param>
            /// <returns>返回符合的特征码的第一个数的地址指针</returns>
            public static IntPtr MemoryQuery(IntPtr startAddress, int pid, int[] memoryBlock)
            {
                int val = 0;
                IntPtr naddr = startAddress,
                       retaddr = IntPtr.Zero;
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);

                while (true)
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    ReadProcessMemory(hProcess, naddr, byteAddress, 1, IntPtr.Zero);

                    if (Marshal.ReadInt32(byteAddress) == memoryBlock[val])
                    {
                        if (val == 0)
                        {
                            retaddr = naddr;
                        }
                        if (val < memoryBlock.Length - 1)
                        {
                            val++;
                            naddr = IntPtr.Add(naddr, 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        val = 0;
                        naddr = IntPtr.Add(naddr, 1);
                        retaddr = IntPtr.Zero;
                    }
                    if ((naddr.ToInt64() - startAddress.ToInt64()) > 0xFFFFFFFF)
                    {
                        retaddr = IntPtr.Zero;
                        break;
                    }
                }
                CloseHandle(hProcess);
                return retaddr;

            }
        }
    }
}
