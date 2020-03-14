# Tools
我的工具集,C#开发，为开发者提供便捷<br/>

引入命名空间：<br/>
using Tools<br/>
该命名空间下主要使用异常类Error<br/>
如：<br/>
try{}catch(Error e){}<br/>
通过e.Type 和 e.MessageText获取错误信息，该类继承Exception，dll中接受Exception抛出的错误(.GetType() 和 Ex.Message)传给Error的.Type 和 .MessageText<br/>

##Net命名空间：<br/>
using Tools.net;<br/>
###Get类：发送网络Get请求<br/>
Get get = new Get("请求连接");<br/>
get.data = "name=Zhangsan&password=Lisi"; // 字符串格式的请求参数<br/>
get.Getdata(); //返回向服务器请求的数据<br/>
get.GetUrl = "http://www.github.com"  //在构造函数中没有设置请求地址的情况下，可给该变量赋值达到同样的效果
非必要参数：<br/>
get.ContentType = "text/json";<br/>
get.UA = ""// 自定义请求UA<br/>
get.IP="" //代理IP端口 不为空时启用代理<br/>
get.Port=6800 //端口默认6800<br/>
get.CookieStr= "" //Cookie 字符串格式 默认为空，不为空时启用<br/>

###Post类：发送网路Post请求<br/>
使用方法同Get类似<br/>


##命名空间：File<br/>
using Tools.File<br/>

操作.ini配置文件<br/>
InIFile ini = new InIFile("操作文件的路径"); //参数可空<br/>
方法:<br/>
ini.SetFilePath("文件路径"); //没有在构造函数中指定操作文件路径时可用此方法设置<br/>
ini.FilePath = "文件路径";  //或者直接给该属性赋值以达到同样效果<br/>
ini.Write("项","键","值","文件路径[可空]");<br/>
ini.Read("项","键","读取失败时返回值","文件路径[可空]");<br/>
ini.DeleteSection("节点/项","路径[可空]")//删除某节点<br/>
ini.DeleteKey("节点","键","路径[可空]");//删除某键<br/>




