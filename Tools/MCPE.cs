using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tools
{
    namespace Minecraft
    {
        public class MCPEData
        {
            public string returnip;
            public int returnport;
            public string mote;          //服务器介绍1
            public int agreement;     //协议族2
            public string version;       //版本3
            public int online;           //在线玩家数4
            public int maxplayer;        //最大玩家数5
            public string dbname;        //存档名字7
            public string gamemode;      //游戏模式8(1)(19132)(19133)()
            public int portv4;
            public int portv6;
            public int delay;            //延迟

            public MCPEData(string ip, int port, string mote, int agreement, string version, int online, int maxplayer, string dbname, string gamemode, int delay, int portv4 = -1, int portv6 = -1)
            {
                this.returnip = ip;
                this.returnport = port;
                this.mote = mote;
                this.agreement = agreement;
                this.version = version;
                this.online = online;
                this.maxplayer = maxplayer;
                this.dbname = dbname;
                this.gamemode = gamemode;
                this.delay = delay;
                //下面参数可能不赋值 为默认 -1
                this.portv4 = portv4;
                this.portv6 = portv6;

            }
        }

        public class MCPE
        {
            /// <summary>
            /// 探测服务器
            /// </summary>
            /// <param name="ip">服务器IP,v4或v6,域名默认v4</param>
            /// <param name="port">基岩版服务器端口</param>
            /// <returns>返回包含数据的MCPEdata类</returns>
            public static MCPEData Ping(string ip, int port)
            {
                Socket client;

                if (ip.IndexOf(":") != -1)
                {
                    client = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                }
                else
                {
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }

                EndPoint point1 = new IPEndPoint(IPAddress.Parse(ip), port);

                long startTick = DateTime.Now.Ticks;
                client.SendTo(Convert.FromBase64String("AQAAAAAkDRLTAP//AP7+/v79/f39EjRWeA=="), point1);
                while (true)
                {

                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    int length = client.ReceiveFrom(buffer, ref point);//接收数据报
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    message = GetRight(message, "MCPE");
                    string[] data = message.Split(new[] { ";" }, StringSplitOptions.None);


                    if (!int.TryParse(data[2], out int agreement))
                    {
                        agreement = -1;
                    }
                    if (!int.TryParse(data[4], out int online))
                    {
                        online = -1;
                    }
                    if (!int.TryParse(data[5], out int maxplayer))
                    {
                        maxplayer = -1;
                    }
                    if (!(data.Length > 9 && int.TryParse(data[9], out int delayvue)))          //只有原版BDS才存在 这个应该不是延迟,但不确定是什么
                    {
                        //delayvue = (int)(DateTime.Now.Ticks - startTick);
                        delayvue = Convert.ToInt32(DateTime.Now.Ticks - startTick);
                    }
                    if (!(data.Length > 10 && int.TryParse(data[10], out int pov4)))
                    {
                        pov4 = -1;
                    }
                    if (!(data.Length > 11 && int.TryParse(data[11], out int pov6)))
                    {
                        pov6 = -1;
                    }

                    return new MCPEData(ip, port, data[1], agreement, data[3], online, maxplayer, data[7], data[8], delayvue, pov4, pov6);
                }

            }

            static string GetRight(string str, string s)
            {
                string temp = str.Substring(str.IndexOf(s), str.Length - str.Substring(0, str.IndexOf(s)).Length);
                return temp;
            }
        }
    }
}
