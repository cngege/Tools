using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
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
            public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
            [DllImport("kernel32")]
            public static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

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

            /// <summary>
            /// 检查文件路径,文件或路径不存在则创建
            /// </summary>
            /// <param name="_Path"></param>
            private static void CheckPath(String _Path)
            {
                if (Path.GetDirectoryName(_Path) != "" && Directory.Exists(Path.GetDirectoryName(_Path)) == false)
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


        public class FileSys
        {
            /// <summary>
            /// 获取运行路径
            /// </summary>
            /// <returns>返回路径字符串反斜杠结尾</returns>
            public static string GetPath()
            {
                return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            }
        }


        public class Folder
        {

            /// <summary>
            /// 文件夹复制
            /// </summary>
            /// <param name="src">原文件夹[如果不是以\结尾则在目标文件夹下建立同名文件夹]</param>
            /// <param name="dest">目的文件夹</param>
            public static void Copy(string src, string dest)
            {
                DirectoryInfo srcdir = new DirectoryInfo(src);
                string destPath = dest;
                if (dest.LastIndexOf('\\') != (dest.Length - 1))
                {
                    dest += "\\";
                }
                if (src.LastIndexOf('\\') != (src.Length - 1))      //如果要复制文件夹的路径不是以\结尾
                {
                    destPath = dest + srcdir.Name + "\\";
                    src += "\\";
                }

                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                FileInfo[] files = srcdir.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.CopyTo(destPath + file.Name, true);
                }
                DirectoryInfo[] dirs = srcdir.GetDirectories();
                foreach (DirectoryInfo dirInfo in dirs)
                {
                    Copy(dirInfo.FullName, destPath);
                }
            }

            /// <summary>
            /// 拷贝文件夹
            /// </summary>
            /// <param name="srcdir">[复制此文件夹]</param>
            /// <param name="desdir">[复制至此文件夹]如果以\结尾就创建要复制的同名文件夹</param>
            /// <param name="overwrite">是否覆盖</param>
            [Obsolete]
            public static void _Copy(string srcdir, string desdir, bool overwrite)
            {
                string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

                string desfolderdir = desdir;

                if (desdir.LastIndexOf("\\") == (desdir.Length - 1))        //输出参数是不是以 \ 结尾的
                {
                    desfolderdir = desdir + folderName;
                }
                string[] filenames = Directory.GetFileSystemEntries(srcdir);

                foreach (string file in filenames)// 遍历所有的文件和目录
                {
                    if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    {
                        string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                        if (!Directory.Exists(currentdir))
                        {
                            Directory.CreateDirectory(currentdir);
                        }

                        _Copy(file, currentdir, overwrite);
                    }
                    else if (File.Exists(file)) // 否则如果是文件直接copy文件
                    {
                        string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                        srcfileName = desfolderdir + "\\" + srcfileName;

                        if (!Directory.Exists(desfolderdir))
                        {
                            Directory.CreateDirectory(desfolderdir);
                        }
                        if (overwrite || !File.Exists(srcfileName))
                        {
                            File.Copy(file, srcfileName, true);
                        }

                    }
                }
            }


            /// <summary>
            /// 获取指定路径的大小
            /// </summary>
            /// <param name="dirPath">路径</param>
            /// <returns></returns>
            public static long GetDirectorySize(string dirPath)
            {
                long len = 0;
                //判断该路径是否存在（是否为文件夹）
                if (!Directory.Exists(dirPath))
                {
                    //查询文件的大小
                    len = new FileInfo(dirPath).Length;

                }
                else
                {
                    //定义一个DirectoryInfo对象
                    DirectoryInfo di = new DirectoryInfo(dirPath);

                    //通过GetFiles方法，获取di目录中的所有文件的大小
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        len += fi.Length;
                    }
                    //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                    DirectoryInfo[] dis = di.GetDirectories();
                    if (dis.Length > 0)
                    {
                        for (int i = 0; i < dis.Length; i++)
                        {
                            len += GetDirectorySize(dis[i].FullName);
                        }
                    }
                }
                return len;
            }

        }

    }

}
