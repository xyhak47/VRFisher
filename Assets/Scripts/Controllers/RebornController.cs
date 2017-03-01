using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWard.tools;



public class RebornController : MonoBehaviour
{
    static public RebornController Instance = null;
    RebornController()
    {
        Instance = this;
    }

    [HideInInspector]
    public int RebornTime = 1;

    public bool TryReborn(ref string msg)
    {
        try
        {


            //string url = "http://120.26.102.143:80/reborn";
            string url = "http://test.vrimmer.com/reborn";

            RebornData rn = new RebornData();
            RebornResultData res = new RebornResultData();
            ///这里到时候你用我传给你的价格和id，现在只是测试例子
            rn.price = MyServerManager.GamePrice.ToString();
            rn.rebornTime = RebornTime;
            rn.wechatPayId = MyServerManager.WeChatPayId;
            rn.sign = Encrypt_MD.MakeSign(rn.wechatPayId);
            string jasondata = Json_Operation.Write_Json(rn);
            res = (RebornResultData) Json_Operation.Read_Json(Json_Operation.PostJsonData(url, jasondata), res);
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
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        return false;
    }

    private bool beginChoose = false;

    public void ShowRebornUI(bool b)
    {
        beginChoose = b;
        UIController.Instance.ShowRebornUI(b);
    }


    void Update()
    {
        if (!beginChoose) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // cancle, and end game
            GameController.Instance.Pause = true;
            ShowRebornUI(false);
            StartCoroutine(UIController.Instance.ShowGameResult());

            SoundController.Instance.PlayMusic(Config.UI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // sure
            string msg = "";
            if(TryReborn(ref msg))
            {
                ShowRebornUI(false);
                Invoke("ResumeGame", 2.0f);
            }
            else
            {
                GameController.Instance.Pause = true;
                ShowRebornUI(false);
                StartCoroutine(UIController.Instance.ShowMsg(msg));
                StartCoroutine(UIController.Instance.ShowGameResult(4));
            }

            SoundController.Instance.PlayMusic(Config.UI);
        }
    }

    private void ResumeGame()
    {
        GameController.Instance.ResetGame();
    }

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

}




