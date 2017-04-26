using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Timers;

public class UIController : MonoBehaviour
{
    static public UIController Instance = null;
    UIController()
    {
        Instance = this;
    }

    [SerializeField]
    private ScoreUI scoreUI;

    [SerializeField]
    private Text[] bulluetCosts;

    [SerializeField]
    private Text message;

    [SerializeField]
    private GameObject rebornUI;

    [SerializeField]
    private GameObject resultUI;

    [SerializeField]
    private Text resultScore;

    [SerializeField]
    private Text config;

    [SerializeField]
    private GameObject UIBOSS;

    [SerializeField]
    private GameObject UItutorial;

    [SerializeField]
    private GameObject UIQuit;

    [SerializeField]
    private RankUI rankUI;

    [SerializeField]
    private GameObject versionUI;

    private bool IsNormalGame;


    public void UpdateScore()
    {
        scoreUI.UpdateScore(GameController.Instance.GunScore, GameController.Instance.FishKilledScore);
    }

    public void SetBulletCost(BulletData bullet)
    {
        bulluetCosts[bullet.Id].text = bullet.Cost.ToString();
    }

    public void SetConfig(string c)
    {
        config.text = c;
    }

    public void Init()
    {
        BulletData bullet = CsvParser.Instance.List_BulletData[0];
        SetBulletCost(bullet);

        UpdateScore();
        scoreUI.SetScoreUIType(GameController.Instance.IsNormalGame);
    }

    public IEnumerator ShowMsg(string msg)
    {
        message.transform.parent.gameObject.active = true;
        message.text = msg;
        yield return new WaitForSeconds(3);
        message.transform.parent.gameObject.active = false;
    }

    public void ShowRebornUI(bool b)
    {
        rebornUI.gameObject.active = b;

        if(b)
        {
            string rebornMsg = "是否尝试花费\n" + MyServerManager.GamePrice + "积分\n用于复活？";
            rebornUI.GetComponent<RebornUI>().WriteMsg(rebornMsg);
            SoundController.Instance.PlayMusic(Config.Fail);
        }
    }

    public IEnumerator ShowGameResult(float wait = 0)
    {
        yield return new WaitForSeconds(wait);

        if (GameController.Instance.IsNormalGame)
        {
            // rank ui
            rankUI.UpdateRankingAndShow(GameController.Instance.FishKilledScore);
        }
        else
        {
            resultUI.gameObject.active = true;
            resultScore.text = GameController.Instance.GunScore.ToString();
        }


        // sound
        SoundController.Instance.PlayMusic(Config.GameOver);

        yield return new WaitForSeconds(GameController.Instance.EndGameWaitSeconds);
        GameController.Instance.EndGame();
    }

    public IEnumerator ShowBoss()
    {
        UIBOSS.gameObject.active = true;
        SoundController.Instance.PlayMusic(Config.Warnig);
        yield return new WaitForSeconds(2);
        UIBOSS.gameObject.active = false;
    }

    public IEnumerator ShowTutorial(float wait)
    {
        UItutorial.gameObject.active = true;
        yield return new WaitForSeconds(wait);
        UItutorial.gameObject.active = false;
    }

    public void ShowQuit(bool q)
    {
        UIQuit.gameObject.active = q;
    }

    public IEnumerator VersionWrong()
    {
        versionUI.gameObject.active = true;
        yield return new WaitForSeconds(10.0f);
        GameController.Instance.EndGame();
    }

    public void PlayerGetAttacked()
    {
        scoreUI.PlayerGetAttacked();
    }

    public void OctopusAttakBegin()
    {
        scoreUI.OctopusAttakBegin();
    }

    public void OctopusAttakEnd()
    {
        scoreUI.OctopusAttakEnd();
    }
}
