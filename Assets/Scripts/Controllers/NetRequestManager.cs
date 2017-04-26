using UnityEngine;
using System.Collections;
using MyWard.tools;

public class NetRequestManager : MonoBehaviour
{
    static public NetRequestManager Instance = null;
    NetRequestManager()
    {
        Instance = this;
    }

    public bool IsTest;

    private string server;
    private const string interface_reborn = "reborn";
    private const string interface_setChest = "setChest";
    private const string interface_checkAppVersion = "checkAppVersion";

    void Awake()
    {
        server = IsTest ? "http://test.vrimmer.com/" : "http://luckstore.vrimmer.com/";
    }

    // ----------------- reborn -------------------------------
    [System.NonSerialized]
    public int RebornTime = 1;

    public bool TryReborn(ref string msg)
    {
        // test
        //RebornTime++;
        //return true;

        try
        {
            string url = server + interface_reborn;

            RebornData rn = new RebornData();
            RebornResultData res = new RebornResultData();
            ///这里到时候你用我传给你的价格和id，现在只是测试例子
            rn.price = MyServerManager.GamePrice.ToString();
            rn.rebornTime = RebornTime;
            rn.wechatPayId = MyServerManager.WeChatPayId;
            rn.sign = Encrypt_MD.MakeSign(rn.wechatPayId);
            string jasondata = Json_Operation.Write_Json(rn);
            res = (RebornResultData)Json_Operation.Read_Json(Json_Operation.PostJsonData(url, jasondata), res);
            if (res != null)
            {
                Debug.Log(res.resultCode + ":::::" + res.resultMessage);
                msg = res.resultMessage;

                if (res.resultCode == "1")
                {
                    RebornTime++;
                    return true;
                }

                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        return false;
    }

    // data class
    private class RebornData
    {
        public string wechatPayId { get; set; }
        public string price { get; set; }
        public int rebornTime { get; set; }
        public string sign { get; set; }
    }

    private class RebornResultData
    {
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
    }


    // ---------------------- box -------------------------
    [System.NonSerialized]
    private int boxTime = 1;

    public bool TryBox(ref string msg, int boxType)
    {
        try
        {
            string url = server + interface_setChest;

            BoxData data = new BoxData();
            BoxResultData res = new BoxResultData();
            ///这里到时候你用我传给你的价格和id，现在只是测试例子
            data.chestType = boxType;
            data.time = boxTime;
            data.wechatPayId = MyServerManager.WeChatPayId;
            data.sign = Encrypt_MD.MakeSign(data.wechatPayId + data.chestType.ToString());
            string jasondata = Json_Operation.Write_Json(data);
           // print(jasondata);
            res = (BoxResultData)Json_Operation.Read_Json(Json_Operation.PostJsonData(url, jasondata), res);
            if (res != null)
            {
                Debug.Log(res.resultCode + ":::::" + res.resultMessage);
                msg = res.resultMessage;

                if (res.resultCode == "1")
                {
                    msg = "获取宝箱";
                    boxTime++;
                    return true;
                }

                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        return false;
    }

    private class BoxData
    {
        public string wechatPayId { get; set; }
        public int chestType { get; set; }
        public int time { get; set; }
        public string sign { get; set; }
    }

    private class BoxResultData
    {
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
    }



    // --------------------- check version-------------
    public void CheckVersion(string version)
    {
        //// test
        //return;

        try
        {
            string url = server + interface_checkAppVersion;

            VersionData v = new VersionData();
            VersionResultData res = new VersionResultData();

            v.appVersion = version;
            v.wechatPayId = MyServerManager.WeChatPayId;
            v.sign = Encrypt_MD.MakeSign(v.wechatPayId);

            string jasondata = Json_Operation.Write_Json(v);
            print(jasondata);

            res = (VersionResultData)Json_Operation.Read_Json(Json_Operation.PostJsonData(url, jasondata), res);
            if (res != null)
            {
                Debug.Log(res.resultCode + ":::::" + res.resultMessage);

                if (res.resultCode == "1")
                {
                    GameController.Instance.ReadyToGame();
                }
                else
                {
                    StartCoroutine( UIController.Instance.VersionWrong() );
                }
            }
        }
        catch (System.Exception ex)
        {
            GameController.Instance.EndGame();
            Debug.Log(ex.ToString());
        }
    }

    private class VersionData
    {
        public string wechatPayId { get; set; }
        public string appVersion { get; set; }
        public string sign { get; set; }
    }

    private class VersionResultData
    {
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
    }
}
