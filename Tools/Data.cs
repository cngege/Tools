using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tools
{
    //数据处理
    namespace Data
    {
        public class JSON : JObject
        {
            /// <summary>
            /// 将JSON格式的字符串重新反序列化
            /// </summary>
            /// <param name="_json">JSON格式的字符串</param>
            /// <returns></returns>
            public static JSON ToJson(String _json)
            {
                return (JSON)Parse(_json);
            }
        }

        public class Data
        {
            //复制数据到剪贴板
            public static void CopyData(object _data)
            {
                Clipboard.SetDataObject(_data);
            }

            /// <summary>
            /// 复制到剪贴板指定格式的数据
            /// </summary>
            /// <param name="format"></param>
            /// <param name="data">数据</param>
            public static void CopyData(string format,object data)
            {
                Clipboard.SetData(format, data);
            }
        }

    }
}
