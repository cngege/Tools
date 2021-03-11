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
            public string motd;          //服务器介绍1
            public int agreement;     //协议族2
            public string version;       //版本3
            public int online;           //在线玩家数4
            public int maxplayer;        //最大玩家数5
            public string dbname;        //存档名字7
            public string gamemode;      //游戏模式8(1)(19132)(19133)()
            public int portv4;
            public int portv6;
            public int delay;            //延迟

            public MCPEData(string ip, int port, string motd, int agreement, string version, int online, int maxplayer, string dbname, string gamemode, int delay, int portv4 = -1, int portv6 = -1)
            {
                this.returnip = ip;
                this.returnport = port;
                this.motd = motd;
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

        
        public enum PingType
        {
            _domin_tov4,
            _domin_tov6,
            _ipv4,
            _ipv6
        } 

        public class MCPE
        {
            /// <summary>
            /// 通过UDP发送给Minecraft服务端的十进制字节型数据包
            /// </summary>
            public static byte[] Packet = new byte[] { 1, 0, 0, 0, 0, 36, 13, 18, 211, 0, 255, 255, 0, 254, 254, 254, 254, 253, 253, 253, 253, 18, 52, 86, 120 };
            /// <summary>
            /// 通过UDP发送给Minecraft服务端的十六进制字节型数据包
            /// </summary>
            public static byte[] Packetx16 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x24, 0x0d, 0x12, 0xd3, 0x00, 0xff, 0xff, 0x00, 0xfe, 0xfe, 0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0x12, 0x34, 0x56, 0x78 };
            /// <summary>
            /// 通过UDP发送给Minecraft服务端的由base64编码的字节型数据包
            /// </summary>
            public static byte[] Packetb64 = Convert.FromBase64String("AQAAAAAkDRLTAP//AP7+/v79/f39EjRWeA==");

            /// <summary>
            /// 向服务端发送请求包接口
            /// </summary>
            /// <param name="domin">Minecraft域名或ip</param>
            /// <param name="port">Minecraft服务端端口</param>
            /// <param name="type">域名样式</param>
            /// <returns>返回 MCPEData 类型对象</returns>
            public static MCPEData Ping(string domin, int port, PingType type)
            {
                IPAddress ipaddr;

                try
                {
                    switch (type)
                    {
                        case PingType._domin_tov4:
                            IPHostEntry host1 = Dns.GetHostEntry(domin);
                            ipaddr = host1.AddressList[0];
                            return GetData(ipaddr, port);
                        case PingType._domin_tov6:
                            IPHostEntry host2 = Dns.GetHostEntry(domin);
                            ipaddr = host2.AddressList[0];
                            return GetData(ipaddr, port, true);
                        case PingType._ipv4:
                            return GetData(IPAddress.Parse(domin), port);
                        case PingType._ipv6:
                            return GetData(IPAddress.Parse(domin), port, true);
                        default:
                            return null;
                    }
                }
                catch (Exception Ex)
                {
                    throw new Error("Tools.Minecraft.MCPE", Ex.ToString());
                }

            }
            

            /// <summary>
            /// 探测服务器
            /// </summary>
            /// <param name="ip">服务器IP,v4或v6</param>
            /// <param name="port">基岩版服务器端口</param>
            /// <param name="v6">是否是ipv6地址</param>
            /// <returns>返回包含数据的MCPEdata类</returns>
            private static MCPEData GetData(IPAddress ip, int port, bool v6 = false)
            {
                Socket client = new Socket(v6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                int startTick = Environment.TickCount;

                client.SendTo(Packet, new IPEndPoint(ip, port));
                while (true)
                {

                    EndPoint point = new IPEndPoint(v6?IPAddress.IPv6Any: IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    int length = client.ReceiveFrom(buffer, ref point);//接收数据报
                    int endTick = Environment.TickCount;
                    int delayvue = endTick - startTick;

                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    message = GetRight(message, "MCPE");
                    string[] data = message.Split(new[] { ";" }, StringSplitOptions.None);
                    client.Close();

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
                    //if (!(data.Length > 9 && int.TryParse(data[9], out int delayvue)))          //只有原版BDS才存在 这个应该不是延迟,但不确定是什么
                    //{
                    //    delayvue = Environment.TickCount - startTick;
                    //    //delayvue = Convert.ToInt32(DateTime.Now.Ticks - startTick);
                    //}
                    if (!(data.Length > 10 && int.TryParse(data[10], out int pov4)))
                    {
                        pov4 = -1;
                    }
                    if (!(data.Length > 11 && int.TryParse(data[11], out int pov6)))
                    {
                        pov6 = -1;
                    }

                    string[] ipport = point.ToString().Split(new[] {":" }, StringSplitOptions.None);

                    return new MCPEData(ipport[0], int.Parse(ipport[1]), data[1], agreement, data[3], online, maxplayer, data[7], data[8], delayvue, pov4, pov6);
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
