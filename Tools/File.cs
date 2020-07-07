using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    //文件操作
    namespace Fileoperate
    {
        /// <summary>
        /// .ini配置文件类，可读写操作.ini配置文件
        /// </summary>
        public class InIFile
        {
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
            [DllImport("kernel32")]
            private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

            /// <summary>
            /// 该实例默认的配置文件路径，如果操作时没有指定配置文件，则之前必须要将路径赋值到此变量
            /// </summary>
            public String FilePath = String.Empty;

            public InIFile(String _filepath = "")
            {
                if (_filepath != String.Empty)
                {
                    FilePath = _filepath;
                }
            }

            private static void CheckPath(String _Path)
            {
                if (Directory.Exists(Path.GetDirectoryName(_Path)) == false)
                {
                    new DirectoryInfo(Path.GetDirectoryName(_Path)).Create();//如何这个文件的文件夹不存在 则创建一个文件夹 
                }
                if (File.Exists(_Path) == false)
                {
                    File.Create(_Path).Close();//如果文件不存在 则创建这个文件
                }
            }


            /// <summary>
            /// 设置你要操作文件的路径，你要操作之前可以用各种方法设置路径，但是必须要有
            /// </summary>
            /// <param name="_FilePath">字符串 路径</param>
            public void SetFilePath(String _FilePath)
            {
                FilePath = _FilePath;
            }

            /// <summary>
            /// 读取节点文件相信键值内容
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="def">如果读不到则返回该值</param>
            /// <param name="_filePath">节点文件路径 [可空]</param>
            /// <returns></returns>
            public String Read(string section, string key, string def, string _filePath = "")
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, def, sb, 1024, _filePath);
                return sb.ToString();
            }


            /// <summary>
            /// 向配置文件中写入信息
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="value">键值</param>
            /// <param name="_filePath">要操作的路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int Write(string section, string key, string value, string _filePath = "")
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                CheckPath(_filePath);
                return WritePrivateProfileString(section, key, value, _filePath);
            }

            /// <summary>
            /// 删除配置文件节点
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="_filePath">配置文件路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int DeleteSection(string section, string _filePath = "")
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                return Write(section, null, null, _filePath);
            }

            /// <summary>
            /// 删除配置文件键所对应的值
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="_filePath">配置文件路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int DeleteKey(string section, string key, string _filePath)
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                return Write(section, key, null, _filePath);
            }
        }

        public class WriteFile
        {
            //打开的文件句柄
            public FileStream Handle;
            public String Path; //写入的文件路径
            public long Sposition; //开始写的位置，实例化的时候会自动获取，也可手动指定

            public WriteFile(FileStream _Handle)
            {
                Handle = _Handle;
                //Handle = File.OpenWrite(Path);
                Sposition = Handle.Length;  //获取本地原本文件的文件长度
                Handle.Seek(Sposition, SeekOrigin.Current);

            }
            public WriteFile(String _Path)
            {
                Path = _Path;
                if (File.Exists(Path))
                {
                    Handle = File.OpenWrite(Path);
                    Sposition = Handle.Length;  //获取本地原本文件的文件长度
                    Handle.Seek(Sposition, SeekOrigin.Current);
                }
                else
                {
                    Handle = new FileStream(Path, FileMode.Create);
                    Sposition = 0;
                }

            }

            public long GetFileSize()
            {
                if (Handle != null)
                {
                    return Handle.Length;
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            /// 写入数据到文件
            /// </summary>
            /// <param name="bt">写入的字节数组数据</param>
            /// <param name="_length">写入长度</param>
            public void Write(byte[] bt, int _length)
            {
                Handle.Write(bt, 0, _length);
            }

        }



    }



}
