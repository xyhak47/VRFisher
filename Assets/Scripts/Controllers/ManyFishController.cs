using UnityEngine;
using System.Collections;

public class ManyFishController : MonoBehaviour
{
    static public ManyFishController Instance = null;
    ManyFishController()
    {
        Instance = this;
    }

    [System.NonSerialized]
    public bool SpawnAllowed = false;

    [System.NonSerialized]
    public bool AlreadyExist = false;

    public int SpawnTimesLimit;

    [System.NonSerialized]
    public int currentSpawnTime = 0;

    public int FishScoreTimes;

    public bool AllowToSpawn()
    {
        return SpawnAllowed && !AlreadyExist && (currentSpawnTime < SpawnTimesLimit);
    }

    public int GetRewardScore()
    {
        return GameController.Instance.PriceTimes * Random.Range(12, 18);
    }

    public void SpawnOnce()
    {
        currentSpawnTime++;
        //UIController.Instance.SetConfig(currentSpawnTime + "," + StoreController.Instance.FishStore);
    }
}
