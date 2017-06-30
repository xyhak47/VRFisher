using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    static public GameController Instance = null;
    GameController()
    {
        Instance = this;
    }

    [System.NonSerialized]
    public int GunScore = 0;

    [System.NonSerialized]
    public int GameScore = 0;

    [System.NonSerialized]
    public int FishKilledScore = 0;

    [System.NonSerialized]
    public bool Pause = false;

    [System.NonSerialized]
    public float CoinCollectDuration = 0.5f;

    [System.NonSerialized]
    public float EndGameWaitSeconds = 10.0f;

    public float GameBeginDelaySec;

    public bool IsNormalGame;

    [System.NonSerialized]
    public int PriceTimes = 1000;

    [System.NonSerialized]
    public int GameRunningDays = 20;

    [System.NonSerialized]
    public string version = "v2.5";

    void Start()
    {
        Version();

        PrepareForGame();

        Pause = true;


        if(IsNormalGame)
        {
            ReadyToGame();
        }
        else
        {
            // test
            ReadyToGame();
        }
    }

    // when checkversion callback
    public void ReadyToGame()
    {
        StartCoroutine(UIController.Instance.ShowTutorial(GameBeginDelaySec));
        Invoke("BeginGame", GameBeginDelaySec);
    }

    public void BeginGame()
    {
        ResetGame();
    }

    public void EndGame()
    {
        print("EndGame");

        // save data
        StoreController.Instance.SaveStoreDataToLocalFile();

        // normal do not need to send score to server, directly end game
        if(IsNormalGame)
        {
            //MyServerManager.EndGame();
        }
        else
        {
           // MyServerManager.SendGoalToClient(GunScore);
        }
    }

    public void OnServerCallBack(string name)
    {
        if(name.Equals(Config.CallBack_SendGoal))
        {
            MyServerManager.EndGame();
        }
    }

    void OnApplicationQuit()
    {
       // MyServerManager.EndGame();
    }

    // 复活ui
    public void ShowReborn()
    {
        Pause = true;

        // normal game has no reborn function, directly end game
        if (IsNormalGame)
        {
            Pause = true;
            StartCoroutine(UIController.Instance.ShowGameResult());
        }
        else
        {
            RebornController.Instance.ShowRebornUI(true);
        }
    }

    public void ResetGame()
    {
        if(IsNormalGame)
        {
            GameScore = GunScore = 2000;
            MyServerManager.GamePrice = GameScore / PriceTimes;
        }
        else
        {
            GameScore = GunScore = MyServerManager.GamePrice * PriceTimes;
        }

        Pause = false;

        UIController.Instance.Init();

        // normal game do not need to quit
        UIController.Instance.ShowQuit(!IsNormalGame);

        GunController.Instance.ResetBullet();

        SoundController.Instance.PlayMusic(Config.ResetGame);

        WaveController.Instance.ResetWaveSpawnSpot();
    }

    private void PrepareForGame()
    {
         //MyServerManager.GameConnectToServer();
         MyServerManager.WeChatPayId = "770411195054136";
        MyServerManager.GamePrice = 1;

        // hide ui quit button
        UIController.Instance.ShowQuit(false);

        CsvParser.Instance.ParseDataFromCsv();
    }

    public void CheckEndGame()
    {
        if (GunScore <= 0)
        {
            GunScore = 0;
            UIController.Instance.UpdateScore();
            ShowReborn();
        }
    }

    private void Version()
    {
        UIController.Instance.SetConfig(version);

        // test
       // UIController.Instance.SetConfig(version +".8");
    }
}
