using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Tools
{
    //数据处理
    namespace Data
    {
        public class JSON
        {
            /// <summary>
            /// 将JSON格式的字符串重新反序列化
            /// </summary>
            /// <param name="jsonstr">JSON格式的字符串</param>
            /// <returns></returns>
            public static T Parse<T>(String jsonstr)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T>(jsonstr);
            }

            /// <summary>
            /// JSON对象(类对象)序列化为字符串
            /// </summary>
            /// <param name="jsonobj">对象</param>
            /// <returns>返回序列化后的字符串</returns>
            public static String stringify(object jsonobj)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(jsonobj);
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
