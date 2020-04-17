using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Tools
{
    /// <summary>
    /// 错误类
    /// </summary>
    public class Error : Exception 
    {
        public String Type;
        public String MessageText;

        /// <summary>
        /// 用于提供工具箱抛出特定类型的错误
        /// </summary>
        /// <param name="_Type">是哪一种错误</param>
        /// <param name="_Message">错误的具体内容</param>
        public Error(String _Type, String _Message)
        {
            Type = _Type;
            MessageText = _Message;
        }

        public String Gettype()
        {
            return Type;
        }

        public String GetMessage()
        {
            return MessageText;
        }
    }


}