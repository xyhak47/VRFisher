using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class MyServerManager
{
    //复活标志位
    private static bool ResumeFlag = false;
    /// <summary>
    /// 调用此方法来连接客户端；可以在某个物体的Awake()方法中来调用此函数；
    /// </summary>
    public static void GameConnectToServer()
    {
        try
        {
            if (StartConnect())
            {
                StandBy();
                MainTimerInitlize();
            }
        }
        catch (Exception e)
        {
            // Debug.Log(DateTime.Now.ToString("hh:mm:ss tt zz")+e);
        }
    }

    #region unity中需要的方法;
    private static void StandBy()
    {
        //调用此方法来向后台发送“场景已正常开启，正待命”消息
        Send(Command.StartUp);

        //调用ErrorLog脚本来记录游戏运行中抛出的错误和异常
        ErrorLog.Instance.init();
    }

    /// <summary>
    /// 获取此函数返回值，来检查是否可以复活；
    /// 而不是向之前一样在本函数下写复活游戏的函数；
    /// </summary>
    /// <returns></returns>
    public static bool ResumeGame()
    {
        return ResumeFlag;
    }
    public static void StartSuccess()
    {//调用此方法来向后台发送“已正常开始场景”消息
        Send(Command.RunOk);
    }
    public static void EndGame()
    {//调用此方法来向后台发送“场景已结束，请关闭场景”消息
        MainTimer.Enabled = false;
        Send(Command.Over);
        CloseConnect();
        //Debug.Log("已断开连接");
    }

    public static void AskForGameResume()
    {//调用此方法向后台发送继续游戏请求
        ResumeFlag = false;
        Send(Command.AskForGameResume);
    }
    public static void HasResumed()
    {
        ResumeFlag = false;
        Send(Command.HasResumed);
    }

    public static bool SendGoalToClient(int goal)
    {
        SendGoal(goal);
        return true;
    }
    #endregion

    #region unity连接客户端服务器
    private static Socket clientSocket;
    static readonly int port = 1122;
    static readonly IPAddress serverIP = IPAddress.Parse("127.0.0.1");
    private static EndPoint remoteEP;
    static byte[] clientByteData = new byte[1024];
    private static ManualResetEvent connectDone = new ManualResetEvent(false);
    //信号量，控制发送命令的回调函数
    private static ManualResetEvent sendDone = new ManualResetEvent(false);
    private static ManualResetEvent receiveDone = new ManualResetEvent(false);
    private static ManualResetEvent disconnectDone = new ManualResetEvent(false);


    private static System.Timers.Timer MainTimer;
    public static string WeChatPayId;
    public static int GamePrice;


    private static void MainTimerInitlize()
    {
        try
        {
            if (MainTimer != null)
            {
                //Debug.Log("已初始化timer");
                return;
            }
            MainTimer = new System.Timers.Timer(30000);
            MainTimer.AutoReset = true;
            MainTimer.Enabled = true;
            MainTimer.Elapsed += MainTimer_Elapsed;
        }
        catch (Exception e)
        {
            // Debug.Log(e);
        }
    }

    static void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        //throw new NotImplementedException();
        try
        {
            UnityEngine.Debug.Log("发送心跳包");
            Send(Command.HeartBeat);
        }
        catch (Exception)
        {
            //  Debug.Log(DateTime.Now.ToString("hh:mm:ss")+"心跳包发送失败："+e);  
        }
    }

    /// <summary>
    /// 用于放在start()中连接服务器
    /// </summary>
    private static bool StartConnect()
    {
        try
        {
            if (clientSocket != null)
                clientSocket.Close();
            remoteEP = new IPEndPoint(serverIP, port);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!clientSocket.Connected)
            {
                try
                {
                    // CloseConnect();
                    UnityEngine.Debug.Log("游戏场景尚未连接到客户端,开始连接");
                    clientSocket.Connect(remoteEP);
                    // Debug.Log(DateTime.Now.ToString("hh:mm:ss")+@"connect success");
                    clientSocket.BeginReceive(clientByteData, 0, clientByteData.Length, 0, new AsyncCallback(OnReceive),
                        clientSocket);
                    UnityEngine.Debug.Log("已连接至客户端");
                    return true;
                }
                catch (SocketException retryConnectException)
                {
                    //  Debug.Log(retryConnectException.ToString());
                }
            }
            // Debug.Log("初始化错误：已连接到客户端");
            return false;
        }
        catch (Exception ex)
        {
            // Debug.Log(DateTime.Now.ToString("hh:mm:ss tt zz") + "连接客户端出错：" + ex);
            return false;
        }
    }

    /// <summary>
    /// 接收的回调函数
    /// </summary>
    /// <param name="ar"></param>
    private static void OnReceive(IAsyncResult ar)
    {
        try
        {
            int bytesRead = clientSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                ProcessReceiveData(clientByteData);
            }
            else
            {
                // normal disconnect
                return;
            }

            clientSocket.BeginReceive(clientByteData, 0, clientByteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        }
        catch (SocketException ex)
        {
            if (ex.ErrorCode == 10054)
            {
                //	StartConnect();
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 接受到的数据解析
    /// </summary>
    /// <param name="bytesData"></param>
    private static void ProcessReceiveData(byte[] bytesData)
    {
        string msg = System.Text.Encoding.Unicode.GetString(bytesData).Replace("\0", null);
        string[] splitmsg = msg.Split(':');
        if (splitmsg.Length > 1)
        {
            if (splitmsg[0] == "L")
            {
                WeChatPayId = splitmsg[1];
                GamePrice = int.Parse(splitmsg[2]);
                UnityEngine.Debug.Log(msg);

                if(!GameController.Instance.IsNormalGame)
                {
                    // sure get wechat id
                    NetRequestManager.Instance.CheckVersion(GameController.Instance.version);
                }

            }
            return;
        }
        int num = System.BitConverter.ToInt32(bytesData, 0);
        //  Debug.Log (num.ToString ());
        try
        {
            Command _cmd = (Command)num;
        }
        catch (Exception e)
        {
            // Debug.Log (e.ToString ());
        }

        try
        {
            switch ((Command)num)
            {
                case Command.Run://当收到跑游戏命令的时候
                    try
                    {
                        //StartGame ();
                        Send(Command.RunOk);
                    }
                    catch
                    {
                    }
                    break;
                case Command.ResumeGranted:
                    try
                    {
                        ResumeFlag = true;
                        // ResumeGame();
                    }
                    catch
                    {
                    }

                    break;
                //	case Command.Close:
                //			try
                //	{
                //		OverGame();
                //	}
                //	catch{}
                //	break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            //  Debug.Log(ex);
        }
    }

    private static void Send(Command _CMD)
    {
        try
        {
            if (!clientSocket.Connected)
            {
                //   Debug.Log("已断开连接");
                MainTimer.Stop();
                return;
            }
            Byte[] byteData;
            byteData = System.BitConverter.GetBytes((int)_CMD);
            clientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSend), clientSocket);
            sendDone.WaitOne();
            // Debug.Log(DateTime.Now.ToString("hh:mm:ss tt zz") + _CMD.ToString());
        }
        catch (Exception exception)
        {
            //  Debug.Log(DateTime.Now.ToString("hh:mm:ss tt zz")+exception);
        }
    }

    private static void SendGoal(int goal)
    {
        string msg = "g:" + goal.ToString();
        try
        {
            if (!clientSocket.Connected)
            {
                MainTimer.Stop();
                return;
            }
            byte[] byteData = System.Text.Encoding.Unicode.GetBytes(msg);
            clientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OnSendGoal), clientSocket);
            sendDone.WaitOne();
        }
        catch (Exception)
        {

        }
    }

    private static void OnSendGoal(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            sendDone.Set();
            GameController.Instance.OnServerCallBack(Config.CallBack_SendGoal);
        }
        catch (Exception e)
        {
            //   Debug.Log(e.ToString());
        }
    }

    private static void OnSend(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            sendDone.Set();

        }
        catch (Exception e)
        {
            //   Debug.Log(e.ToString());
        }
    }

    private static void CloseConnect()
    {
        try
        {
            if (clientSocket.Connected)
            {
                //     Debug.Log("请求断开连接");
                MainTimer.Stop();
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.BeginDisconnect(true, new AsyncCallback(ClientOnDisconnect), clientSocket);
            }
        }
        catch
        {
            // Debug.Log("断开连接出错");
        }
    }

    private static void ClientOnDisconnect(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndDisconnect(ar);
        }
        catch
        {

        }
    }
    enum Command
    {
        StartUp,//当客户端连接服务器时发送的命令代表游戏已经启动了
        Run,//表示当服务器检测到硬币是所需要的时候发的命令
        RunOk,//表示已经开始启动
              //	Close,//表示提前关闭游戏
        Over,//游戏关闭时发送的命令
        HeartBeat,//心跳包
        ResumeGranted,//同意继续游戏
        AskForGameResume,//请求继续游戏
        HasResumed//新增已经开始复活游戏
    }
}



#endregion


