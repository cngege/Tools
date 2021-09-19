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
            public struct MEMORY_BASIC_INFORMATION
            {
                public IntPtr BaseAddress;
                public IntPtr AllocationBase;
                public int AllocationProtect;
                public int RegionSize;      //区域大小
                public int State;           //状态
                public int Protect;         //保护类型
                public int lType;
            }

            public const int MEM_COMMIT = 0x1000;       //已物理分配
            public const int MEM_PHYSICAL = 0x400000;
            public const int MEM_RESERVE = 0x2000;
            public const int MEM_DECOMMIT = 0x4000;
            public const int MEM_RELEASE = 0x8000;
            public const int MEM_RESET = 0x80000;
            public const int MEM_TOP_DOWN = 0x100000;
            public const int MEM_WRITE_WATCH = 0x200000;

            public const int PAGE_NOACCESS = 0x01;
            public const int PAGE_READONLY = 0x02;      //只读
            public const int PAGE_READWRITE = 0x04;     //可读写内存
            public const int PAGE_WRITECOPY = 0x08;
            public const int PAGE_EXECUTE = 0x10;
            public const int PAGE_EXECUTE_READ = 0x20;
            public const int PAGE_EXECUTE_READWRITE = 0x40;
            public const int PAGE_EXECUTE_WRITECOPY = 0x80;
            public const int PAGE_GUARD = 0x100;
            public const int PAGE_NOCACHE = 0x200;
            public const int PAGE_WRITECOMBINE = 0x400;

            //向远程进程申请一段内存
            [DllImport("kernel32.dll")]
            public static extern IntPtr VirtualAllocEx(IntPtr hwnd, IntPtr lpaddress, int size, int type, int tect);

            //回收申请的内存
            [DllImport("kernel32.dll")]
            public static extern int VirtualFreeEx(IntPtr hwnd, IntPtr lpaddress,int size, int type);

            [DllImport("kernel32.dll")]
            public static extern bool VirtualProtectEx(IntPtr hwnd, IntPtr lpaddress, int dwsize, int flNewProtect,ref int lpflOldProtect);
            //查询内存块信息
            [DllImport("kernel32.dll")]
            public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
            //从指定内存中读取字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

            //从指定内存中写入字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "WriteProcessMemory")]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

            //打开一个已存在的进程对象，并返回进程的句柄
            [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            //关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。
            [DllImport("kernel32.dll")]
            public static extern void CloseHandle(IntPtr hObject);

            //获取模块的基址 null获取本模块(??主程序??)
            [DllImport("Kernel32.dll")]
            public static extern IntPtr GetModuleHandleA(string moudle);

            [DllImport("user32.dll", EntryPoint = "SetWindowText")]
            public static extern int SetWindowText(IntPtr hwnd,string formtitle);


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
                    //return ps.MainModule.EntryPointAddress;
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
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    int refval = ReadValue(address, hProcess, AddrSize);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 32 位带符号整数。
                    return refval;
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 读内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="hProcess">带权限的进程句柄</param>
            /// <param name="AddrSize">读取的内存大小</param>
            /// <returns>返回的内存值</returns>
            public static int ReadValue(IntPtr address, IntPtr hProcess, int AddrSize = 4)
            {
                try
                {
                    byte[] buffer = new byte[AddrSize];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, AddrSize, IntPtr.Zero);
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
            public static long ReadValue64(IntPtr address, int pid, int AddrSize = 8)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                long refval = ReadValue64(address, hProcess,AddrSize);
                //关闭操作
                CloseHandle(hProcess);
                return refval;
            }
            /// <summary>
            /// 读内存长整数
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="hProcess">带权限的进程句柄</param>
            /// <param name="AddrSize">读取的内存大小</param>
            /// <returns>返回的内存值</returns>
            public static long ReadValue64(IntPtr address, IntPtr hProcess, int AddrSize = 8)
            {
                try
                {
                    byte[] buffer = new byte[AddrSize];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, AddrSize, IntPtr.Zero);
                    //从非托管内存中读取一个 64 位带符号整数。
                    return Marshal.ReadInt64(byteAddress);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 读内存单精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public static float ReadValue_float(IntPtr address, int pid)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                //将制定内存中的值读入缓冲区
                float retval =  ReadValue_float(address,hProcess);
                //关闭操作
                CloseHandle(hProcess);
                return retval;
            }

            /// <summary>
            /// 读内存单精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="hProcess"></param>
            /// <returns></returns>
            public static float ReadValue_float(IntPtr address, IntPtr hProcess)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, 4, IntPtr.Zero);
                    //从非托管内存中读取一个 单精度浮点数。
                    return BitConverter.ToSingle(buffer, 0);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 读双精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public static double ReadValue_double(IntPtr address, int pid)
            {
                try
                {
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    double refval = ReadValue_double(address,hProcess);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 双精度浮点数
                    return refval;
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 读双精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="hProcess"></param>
            /// <returns></returns>
            public static double ReadValue_double(IntPtr address, IntPtr hProcess)
            {
                try
                {
                    byte[] buffer = new byte[8];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, 8, IntPtr.Zero);
                    //从非托管内存中读取一个 双精度浮点数
                    return BitConverter.ToDouble(buffer, 0);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 取内存指针
            /// </summary>
            /// <param name="address"></param>
            /// <param name="pid"></param>
            /// <param name="ofs">取前偏移</param>
            /// <returns></returns>
            public static IntPtr ReadValue_IntPtr(IntPtr address, int pid,int ofs = 0)
            {
                try
                {
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    IntPtr byteAddress = ReadValue_IntPtr(address, hProcess, ofs);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 指针
                    return byteAddress;
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }

            /// <summary>
            /// 取内存指针
            /// </summary>
            /// <param name="address"></param>
            /// <param name="hProcess"></param>
            /// <param name="ofs">取前偏移</param>
            /// <returns></returns>
            public static IntPtr ReadValue_IntPtr(IntPtr address, IntPtr hProcess, int ofs = 0)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    if (ofs != 0)
                    {
                        address = IntPtr.Add(address, ofs);
                    }
                    ReadProcessMemory(hProcess, address, byteAddress, 4, IntPtr.Zero);
                    //从非托管内存中读取一个 指针
                    return Marshal.ReadIntPtr(byteAddress);
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }

            /// <summary>
            /// 取内存指针64位
            /// </summary>
            /// <param name="address"></param>
            /// <param name="pid"></param>
            /// <param name="ofs">取前偏移</param>
            /// <returns></returns>
            public static IntPtr ReadValue_IntPtr64(IntPtr address, int pid, int ofs = 0)
            {
                try
                {
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    IntPtr byteAddress =  ReadValue_IntPtr64(address,hProcess,ofs);
                    //关闭操作
                    CloseHandle(hProcess);
                    return byteAddress;
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }

            /// <summary>
            /// 取内存指针64位
            /// </summary>
            /// <param name="address"></param>
            /// <param name="hProcess"></param>
            /// <param name="ofs">取前偏移</param>
            /// <returns>在32位系统只能返回前四个字节的指针</returns>
            public static IntPtr ReadValue_IntPtr64(IntPtr address, IntPtr hProcess, int ofs = 0)
            {
                try
                {
                    byte[] buffer = new byte[8];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    if (ofs != 0)
                    {
                        address = IntPtr.Add(address, ofs);
                    }
                    ReadProcessMemory(hProcess, address, byteAddress, 8, IntPtr.Zero);
                    //从非托管内存中读取一个 指针
                    return Marshal.ReadIntPtr(byteAddress);
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }

            /// <summary>
            /// 读内存字节数组
            /// </summary>
            /// <param name="address"></param>
            /// <param name="pid"></param>
            /// <param name="len"></param>
            /// <returns></returns>
            public static byte[] ReadValue_bytes(IntPtr address, int pid, int len)
            {
                try
                {
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    byte[] buffer = ReadValue_bytes(address,hProcess,len);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 指针
                    return buffer;
                }
                catch
                {
                    return new byte[] { };
                }
            }

            /// <summary>
            /// 读内存字节数组
            /// </summary>
            /// <param name="address"></param>
            /// <param name="hProcess"></param>
            /// <param name="len"></param>
            /// <returns></returns>
            public static byte[] ReadValue_bytes(IntPtr address, IntPtr hProcess, int len)
            {
                try
                {
                    byte[] buffer = new byte[len];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, address, byteAddress, len, IntPtr.Zero);
                    //从非托管内存中读取一个 指针
                    return buffer;
                }
                catch
                {
                    return new byte[] { };
                }
            }

            /// <summary>
            /// 写单精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="value"></param>
            /// <param name="pid"></param>
            public static void WriteValue_float(IntPtr address, float value, int pid)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                WriteValue_float(address, value, hProcess);
                //关闭操作
                CloseHandle(hProcess);
            }

            /// <summary>
            /// 写单精度浮点数
            /// </summary>
            /// <param name="address"></param>
            /// <param name="value"></param>
            /// <param name="hProcess"></param>
            public static void WriteValue_float(IntPtr address, float value, IntPtr hProcess)
            {
                int virtue = 0;
                bool b = VirtualProtectEx(hProcess, address, sizeof(float), PAGE_READWRITE, ref virtue);
                byte[] _byte = BitConverter.GetBytes(value);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, address, _byte, 4, IntPtr.Zero);
                if (b)
                {
                    VirtualProtectEx(hProcess, address, sizeof(float), virtue, ref virtue);
                }
            }
            /// <summary>
            /// 写内存字节集
            /// </summary>
            /// <param name="address">要写入的地址</param>
            /// <param name="value">写入的字节数组</param>
            /// <param name="pid">进程pid</param>
            public static void WriteValue_bytes(IntPtr address, byte[] value, int pid)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                WriteValue_bytes(address, value, hProcess);
                //关闭操作
                CloseHandle(hProcess);
            }

            /// <summary>
            /// 写内存字节集
            /// </summary>
            /// <param name="address">要写入的地址</param>
            /// <param name="value">写入的字节数组</param>
            /// <param name="hProcess">具有权限的进程句柄</param>
            public static void WriteValue_bytes(IntPtr address, byte[] value, IntPtr hProcess)
            {
                int virtue = 0;
                bool b = VirtualProtectEx(hProcess, address, value.Length, PAGE_READWRITE, ref virtue);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, address, value, value.Length, IntPtr.Zero);
                if (b)
                {
                    VirtualProtectEx(hProcess, address, value.Length, virtue, ref virtue);
                }
                
            }

            /// <summary>
            /// 写内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="value">写入的内存值</param>
            /// <param name="pid">进程pid</param>
            /// <param name="AddrSize">写入的地址大小[有点多余]</param>
            public static void WriteValue(IntPtr address, int value, int pid, int AddrSize = 4)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                WriteValue(address,value, hProcess,AddrSize);
                //关闭操作
                CloseHandle(hProcess);
            }

            /// <summary>
            /// 写内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="value">写入的内存值</param>
            /// <param name="hProcess">具有权限的进程句柄</param>
            /// <param name="AddrSize">写入的地址大小[有点多余]</param>
            public static void WriteValue(IntPtr address, int value, IntPtr hProcess, int AddrSize = 4)
            {
                int virtue = 0;
                bool b = VirtualProtectEx(hProcess, address, sizeof(int), PAGE_READWRITE, ref virtue);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, address, BitConverter.GetBytes(value), AddrSize, IntPtr.Zero);
                if (b)
                {
                    VirtualProtectEx(hProcess, address, sizeof(int), virtue, ref virtue);
                }
            }

            public static IntPtr ApplyRunMemory(int pid,int size)
            {
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                IntPtr lpAddress = ApplyRunMemory(hProcess, size);
                //int virtue = 0;
                //VirtualProtectEx(hProcess, lpAddress, 0x500, PAGE_EXECUTE_READWRITE, ref virtue);
                CloseHandle(hProcess);
                return lpAddress;
            }

            public static IntPtr ApplyRunMemory(IntPtr hProcess, int size)
            {
                IntPtr lpAddress = VirtualAllocEx(hProcess, IntPtr.Zero, size, MEM_COMMIT | MEM_TOP_DOWN, PAGE_EXECUTE_READWRITE);
                return lpAddress;
            }

            /// <summary>
            /// 内存搜索 支持模糊搜索
            /// </summary>
            /// <param name="pid"></param>
            /// <param name="memoryBlock"></param>
            /// <param name="moudlename">要内存搜索的模块名,如果空则为主程序</param>
            /// <returns>返回匹配的内存地址数组</returns>
            public static IntPtr[] MemoryQuery(int pid, int[] memoryBlock, string moudlename = null)
            {
                Process ps = Process.GetProcessById(pid);
                IntPtr startAddress = IntPtr.Zero;
                int moudlesize = 0;
                if (moudlename == null)
                {
                    startAddress = ps.MainModule.BaseAddress;
                    moudlesize = ps.MainModule.ModuleMemorySize;
                }
                else
                {
                    for (int i = 0; i < ps.Modules.Count; i++)
                    {
                        if (ps.Modules[i].ModuleName == moudlename)
                        {
                            startAddress = ps.Modules[i].BaseAddress;
                            moudlesize = ps.Modules[i].ModuleMemorySize;
                            break;
                        }
                    }
                    if (moudlesize == 0)
                    {
                        ps.Close();
                        return new IntPtr[] { };
                    }
                }
                ps.Close();
                
                IntPtr naddr = startAddress;
                List<IntPtr> value = new List<IntPtr>();
                try
                {
                    //int count = 4096000;
                    int count = moudlesize;

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
                                        if (memoryBlock[j] != -1 && memoryBlock[j] != bffarray[i + j])  //!= -1 模糊搜索
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (j == memoryBlock.Length - 1)    // 表示符合的已经是数组最后一个了 表示全符合
                                            {
                                                //找到了
                                                value.Add(IntPtr.Add(naddr, i));
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
                                        if (i + j <= count - 1)                             //还没超出第一个数组范围
                                        {
                                            if (memoryBlock[j] != -1 && memoryBlock[j] != bffarray[i + j])
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (j == memoryBlock.Length - 1)
                                                {
                                                    //找到了
                                                    value.Add(IntPtr.Add(naddr, i));
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
                                            if (memoryBlock[j] != -1 && memoryBlock[j] != bffarray2[j - (count - 1 - i)])
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (j == memoryBlock.Length - 1)
                                                {
                                                    //找到了
                                                    value.Add(IntPtr.Add(naddr, i));
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        //if (naddr.ToInt64() > 0x7FFFFFFFFFFFFFFF)
                        if ((naddr.ToInt64() - startAddress.ToInt64()) > moudlesize)
                        {
                            //System.Windows.Forms.MessageBox.Show(((Int64)IntPtr.Add(startAddress,moudlesize)).ToString("x16"));
                            CloseHandle(Process);
                            return value.ToArray();
                        }
                        naddr = IntPtr.Add(naddr, count);
                    }
                }
                catch (Exception e)
                {
                    //System.Windows.Forms.MessageBox.Show(e.StackTrace + "\n" + e.Message);
                    return value.ToArray();
                }
            }

            /// <summary>
            /// 将 0F 00 05 11 ?? FF 此类的16进制字符串转化为int数组,以便内存查询
            /// </summary>
            /// <param name="memory"></param>
            /// <returns></returns>
            public static int[] Memory_Parse(string memory)
            {
                string[] moy = memory.Split(' ');
                int[] intmoy = new int[moy.Length];
                for (int i = 0; i < moy.Length; i++)
                {
                    if (moy[i] == "??" || moy[i] == "?")
                    {
                        intmoy[i] = -1;
                    }
                    else
                    {
                        intmoy[i] = Convert.ToInt32("0x"+moy[i],16);
                    }
                }
                return intmoy;
            }

            /// <summary>
            /// 将 0F 00 05 11 ?? FF 此类的16进制字符串转化为字节数组,以便内存查询
            /// </summary>
            /// <param name="memory"></param>
            /// <returns></returns>
            public static byte[] Memory_byteParse(string memory)
            {
                string[] moy = memory.Split(' ');
                byte[] intmoy = new byte[moy.Length];
                for (int i = 0; i < moy.Length; i++)
                {
                    if (moy[i] == "??" || moy[i] == "?")
                    {
                        throw new Error("Address.Address.Memory_Parse","反序列化特征码到字节数组不支持模糊字符[??]");
                    }
                    else
                    {
                        intmoy[i] = (byte)Convert.ToInt32("0x" + moy[i], 16);
                    }
                }
                return intmoy;
            }

        }
    }
}
