using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    static public GameController Instance = null;
    GameController()
    {
        Instance = this;
    }

    public GameObject coin;
    public float PumpingRate = 0.05f; // 10%
    public int GunScore = 0;

    [HideInInspector]
    public bool Pause = false;

    public float CoinCollectDuration = 0.5f;

    public float EndGameWaitSeconds = 10.0f;

    public Color fishBlinkColor;


    public void KillFish(Fish fish)
    {
        Instantiate(coin, fish.transform.position, Quaternion.identity);
    }

    void Start()
    {
        PrepareForGame();

        Pause = true;
        Invoke("BeginGame", 1.0f);
    }

    public void BeginGame()
    {
       ResetGame();

       //test();
    }

    public void EndGame()
    {
        print("EndGame");
        MyServerManager.SendGoalToClient(GunScore);

        MyServerManager.EndGame();
    }

    // 复活ui
    public void ShowReborn()
    {
        Pause = true;
        RebornController.Instance.ShowRebornUI(true);
    }

    public void ResetGame()
    {
        GunScore = MyServerManager.GamePrice * 1000;
        //GunScore = 1000;
        Pause = false;

        WaveController.Instance.ResetCurrentWave();
        WaveController.Instance.BeginWave();

        UIController.Instance.Init();

        SoundController.Instance.PlayMusic(Config.ResetGame);
    }

    private void PrepareForGame()
    {
        MyServerManager.GameConnectToServer();
       // MyServerManager.WeChatPayId = "14770216100836115";
       //MyServerManager.GamePrice = 5;

        CsvParser.Instance.ParseDataFromCsv();
    }

    private void test()
    {
        // test
        UIController.Instance.SetConfig("WeChatPayId=" + MyServerManager.WeChatPayId + "\n" + "GamePrice=" + MyServerManager.GamePrice);
    }
}
