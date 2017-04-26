using UnityEngine;
using System.Collections;
using Custom;

public class StoreController : MonoBehaviour
{
    static public StoreController Instance = null;
    StoreController()
    {
        Instance = this;
    }

    private Timer timer = new Timer();

    public float StoreResetDeltaSec;

    [System.NonSerialized]
    public int FishStore = 0;

    void Start()
    {
        timer.tick += OnTimerEvent;
        timer.ResetTriggerTime(StoreResetDeltaSec);
        timer.Restart();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);
    }

    private void OnTimerEvent(Timer that)
    {
        ResetFishStore();
        CheckChangeStoreEffect();

        timer.Restart();
    }

    void OnDestroy()
    {
        timer.Stop();
    }

    private void ResetFishStore()
    {
        int lastGameFishStore = PlayerPrefabWithXXTEA.GetInt(Config.FishStoreKey);

        FishStore = lastGameFishStore + 
            (NetRequestManager.Instance.RebornTime * GameController.Instance.GameScore) - 
            GameController.Instance.GunScore;

        // reset many fish AlreadyExist in per store delta time
        ManyFishController.Instance.AlreadyExist = false;

        // test : show fish store to ui
       // UIController.Instance.SetConfig(ManyFishController.Instance.currentSpawnTime + "," + StoreController.Instance.FishStore);
    }

    private void CheckChangeStoreEffect()
    {
        int compareValue = GameController.Instance.PriceTimes * GameController.Instance.GameRunningDays;
        ManyFishController.Instance.SpawnAllowed = FishStore <= compareValue ? false : true;

        // test
        ManyFishController.Instance.SpawnAllowed = true;
    }

    public void SaveStoreDataToLocalFile()
    {
        PlayerPrefabWithXXTEA.SetInt(Config.FishStoreKey, FishStore);
        PlayerPrefabWithXXTEA.Save();
    }
}
