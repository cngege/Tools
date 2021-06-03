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

            //获取模块的基址 null获取本模块(??主程序??)
            [DllImport("Kernel32.dll")]
            public static extern IntPtr GetModuleHandleA(string moudle);

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
            /// 获取模块地址
            /// </summary>
            /// <param name="pid">有效的pid</param>
            /// <param name="module">为空时获取主exe程序的基址</param>
            /// <returns></returns>
            public static IntPtr GetModuleAddr(int pid, string module = null)
            {
                Process ps = Process.GetProcessById(pid);
                if (module == null)
                {
                    return ps.MainModule.BaseAddress;
                }
                else
                {
                    for (int i = 0; i < ps.Modules.Count; i++)
                    {

                        if (ps.Modules[i].ModuleName == module)
                        {
                            return ps.Modules[i].BaseAddress;
                        }
                    }
                    return IntPtr.Zero;
                }
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
                    byte[] buffer = new byte[AddrSize];
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
                    byte[] buffer = new byte[AddrSize];
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
            /// 内存快速搜索特征码
            /// 考虑在线程中操作
            /// </summary>
            /// <param name="startAddress">开始查询地址,一般为基址</param>
            /// <param name="pid">程序Pid</param>
            /// <param name="memoryBlock">特征码</param>
            /// <returns>返回符合的特征码的第一个数的地址指针</returns>
            public static IntPtr MemoryQuery(IntPtr startAddress, int pid, int[] memoryBlock)
            {
                IntPtr naddr = startAddress;
                try
                {
                    int count = 20480;

                    byte[] bffarray = new byte[count];
                    IntPtr bffAddress = Marshal.UnsafeAddrOfPinnedArrayElement(bffarray, 0);
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr Process = OpenProcess(0x1F0FFF, false, pid);
                    //将制定内存中的值读入缓冲区

                    while (true)
                    {

                        ReadProcessMemory(Process, naddr, bffAddress, count, IntPtr.Zero);   //读取并存入缓冲区
                        for (int i = 0; i < bffarray.Length; i++)
                        {
                            if (bffarray[i] == memoryBlock[0])
                            {
                                if (count - i >= memoryBlock.Length)     //不是在bffarray末尾匹配到的
                                {
                                    for (int j = 0; j < memoryBlock.Length; j++)
                                    {
                                        if (memoryBlock[j] != bffarray[i + j])
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (j == memoryBlock.Length - 1)
                                            {
                                                CloseHandle(Process);
                                                //找到了
                                                return IntPtr.Add(naddr, i);
                                            }
                                        }
                                        //到这里表示 找到了第一个值 但后面的值不匹配
                                    }
                                }
                                else
                                {
                                    //表示 这是在bffarray最后几个元素中找到的 确定了匹配的值不够
                                    byte[] bffarray2 = new byte[memoryBlock.Length];
                                    IntPtr bffAddress2 = Marshal.UnsafeAddrOfPinnedArrayElement(bffarray2, 0);
                                    ReadProcessMemory(Process, IntPtr.Add(naddr, count), bffAddress2, memoryBlock.Length, IntPtr.Zero);   //读取并存入缓冲区

                                    for (int j = 0; j < memoryBlock.Length; j++)
                                    {
                                        if (i + j <= count - 1)
                                        {
                                            if (memoryBlock[j] != bffarray[i + j])
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (j == memoryBlock.Length - 1)
                                                {
                                                    CloseHandle(Process);
                                                    //找到了
                                                    return IntPtr.Add(naddr, i);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //1 2 3 4 5
                                            //          6 7 8 9 10
                                            //  2 3 4 5 6 7
                                            //count = 6
                                            //i = 1   j - (count - i - 1)
                                            //j = 4   4 - (6 - 1 - 1) = 0
                                            //    6 == 6 true
                                            //j = 5   5 - (6 - 1 - 1) = 1
                                            //    7 == 7 true
                                            if (memoryBlock[j] != bffarray2[j - (count - 1 - i)])
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (j == memoryBlock.Length - 1)
                                                {
                                                    CloseHandle(Process);
                                                    //找到了
                                                    return IntPtr.Add(naddr, i);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if ((naddr.ToInt64() - startAddress.ToInt64()) > 0xFFFFFFFF)
                        {
                            CloseHandle(Process);
                            return IntPtr.Zero;
                        }
                        naddr = IntPtr.Add(naddr, count);
                    }
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }
        }
    }
}
