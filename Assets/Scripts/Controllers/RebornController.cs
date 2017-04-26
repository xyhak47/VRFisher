using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class RebornController : MonoBehaviour
{
    static public RebornController Instance = null;
    RebornController()
    {
        Instance = this;
    }

    private bool beginChoose = false;

    [System.NonSerialized]
    public bool animationCompleted = false;

    public void ShowRebornUI(bool b)
    {
        beginChoose = b;
        UIController.Instance.ShowRebornUI(b);
    }

    void Update()
    {
        if (!beginChoose) return;
        if (!animationCompleted) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("CancleReborn");

            // cancle, and end game
            CancleReborn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // sure
            print("TryToReborn");
            TryToReborn();
        }
    }

    private void ResumeGame()
    {
        GameController.Instance.ResetGame();
    }

    public void CancleReborn()
    {
        GameController.Instance.Pause = true;
        ShowRebornUI(false);
        StartCoroutine(UIController.Instance.ShowGameResult());
       // GameController.Instance.EndGame();

        SoundController.Instance.PlayMusic(Config.UI);
    }

    private void TryToReborn()
    {
        string msg = "";
        if (NetRequestManager.Instance.TryReborn(ref msg))
        {
            ShowRebornUI(false);
            Invoke("ResumeGame", 2.0f);
        }
        else
        {
            GameController.Instance.Pause = true;
            ShowRebornUI(false);
            StartCoroutine(UIController.Instance.ShowMsg(msg));

            //StartCoroutine(waitToEndGame(3));
            StartCoroutine(UIController.Instance.ShowGameResult(4));
        }

        SoundController.Instance.PlayMusic(Config.UI);
    }

    private IEnumerator waitToEndGame(float sec)
    {
        yield return new WaitForSeconds(sec);
        GameController.Instance.EndGame();
    }
}




