using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Custom;

public class WaveController : MonoBehaviour
{
    static public WaveController Instance = null;
    WaveController()
    {
        Instance = this;
    }

    public GameObject coin;

    [System.NonSerialized]
    public Wave CurrentWave = null;

    private bool HasNextWave = true;

    [System.NonSerialized]
    public List<Fish> fishBag = new List<Fish>();

    // fish effect
    public GameObject effect_Boom;
    public GameObject effect_Ice;
    public GameObject effect_Many;

    [System.NonSerialized]
    public bool InIceEffect = false;

    public Color fishBlinkColor;
    public Color fishFreezeColor;

    public float fishCrowedSpawnDelaySec;

    // function fish effect properties
    public float iceFishEffect_freezeSecond;

    [System.NonSerialized]
    public float RateWeight = 1.1f;

    public void ResetWaveSpawnSpot()
    {
        // reset wave
        ResetCurrentWave();

        StopAllCoroutines();

        //int spawnSpot = Mathf.FloorToInt(Mathf.Log(NetRequestManager.Instance.RebornTime * MyServerManager.GamePrice / 3.0f + 1, 5));
        //spawnSpot = Mathf.Clamp(spawnSpot, 1, 2);
        int spawnSpot = 1;

        for (int i = 0; i < spawnSpot; i++)
        {
            BeginWave();
        }
    }

    private void BeginWave()
    {
        StartCoroutine(NextFish());
    }

    IEnumerator NextFish()
    {
        foreach (FishData fishData in CsvParser.Instance.GetRandomFishFromCurrentWave())
        {
            SpawnFish(fishData);
            yield return new WaitForSeconds(CurrentWave.Delay);
        }

        yield return new WaitForSeconds(CurrentWave.WaveDelay);

        if (HasNextWave)
        {
            NextWave();
            BeginWave();
        }
        else
        {
            ResetCurrentWave();
            BeginWave();
        }
    }

    private void ResetCurrentWave()
    {
        Assert.IsTrue(CsvParser.Instance.List_Wave.Count != 0, "List_Wave is empty!");

        CurrentWave = CsvParser.Instance.List_Wave[0];
        HasNextWave = true;
    }

    private void NextWave()
    {
        int next = CurrentWave.Id + 1;
        CurrentWave = CsvParser.Instance.List_Wave[next];
        //next = Mathf.Clamp(next, 0, CsvParser.Instance.List_Wave.Count - 1);
        HasNextWave = next >= CsvParser.Instance.List_Wave.Count - 1 ? false : true;
    }

    public Fish SpawnFish(int id)
    {
        return SpawnFish(CsvParser.Instance.FindFish(id, false));
    }

    private Fish SpawnFish(FishData fishData)
    {
        GameObject fish = Resources.Load(Config.FishPrefabFolder + fishData.Id) as GameObject;
        Assert.IsTrue(fish != null, "Resources.Load fish prefab null, fish id is " + fishData.Id);
        fish = Instantiate(fish, Vector3.zero, Quaternion.identity) as GameObject;

        // origin fish
        GameObject pathPrefab = PathController.Instance.GetRandomPath(fishData);
        GameObject path = Instantiate(pathPrefab);
        fish.transform.parent = path.transform;
        fish.transform.localPosition = Vector3.zero;
        fish.transform.localEulerAngles = new Vector3(0, -90, 0);
        fish.GetComponent<Fish>().AttachFishData(fishData);

        if (fish.GetComponent<Fish>().isWithCrowed)
        {
            StartCoroutine(SpawnFishCrowed(fish.GetComponent<Fish>(), pathPrefab));
        }

        return fish.GetComponent<Fish>();
    }

    private IEnumerator SpawnFishCrowed(Fish modelFish, GameObject modelPathPrefab)
    {
        Transform modelFishTransform = modelFish.gameObject.transform;
        int modelFishId = modelFish.fishData.Id;
        FishData modelFishData = modelFish.fishData;

        // fish crowed
        foreach (var offsetPos in modelFish.fishCrowedoffsets)
        {
            GameObject subFish = Resources.Load(Config.FishPrefabFolder + modelFishId) as GameObject;
            subFish = Instantiate(subFish, Vector3.zero, Quaternion.identity) as GameObject;
            subFish.GetComponent<Fish>().isWithCrowed = false;
            subFish.GetComponent<Fish>().AttachFishData(modelFishData);

            GameObject path = Instantiate(modelPathPrefab);
            subFish.transform.parent = path.transform;
            subFish.transform.localPosition = modelFishTransform.localPosition + offsetPos;
            subFish.transform.localEulerAngles = new Vector3(0, -90, 0);

            yield return new WaitForSeconds(fishCrowedSpawnDelaySec);
        }
    }

    public void KillFish(Fish fish, GameObject net)
    {
        Instantiate(coin, fish.transform.position, Quaternion.identity);

        // check special fish effect
        if (fish.IsFunctionFish(Fish.FunctionFish.IceFish))
        {
            StartCoroutine(ShowIceFishDieEffect(fish.gameObject));
        }
        else if (fish.IsFunctionFish(Fish.FunctionFish.BoomFish))
        {
            ShowBoomFishDieEffect(fish.gameObject, net);
        }
        else if (fish.IsFunctionFish(Fish.FunctionFish.ManyFish))
        {
            StartCoroutine(ShowManyFishDieEffect(fish.gameObject));
        }
        else if (fish.IsFunctionFish(Fish.FunctionFish.Octopus))
        {
            ShowOctopusDieEffect();
        }
        else
        {
            SoundController.Instance.PlayMusic(Config.FishDie);
        }
    }

    private IEnumerator ShowIceFishDieEffect(GameObject iceFish)
    {
        Instantiate(effect_Ice, iceFish.transform.position, iceFish.transform.rotation);
        SoundController.Instance.PlayMusic(Config.Ice);

        InIceEffect = true;
        CameraController.Instance.PostEffect_IceStart();
        foreach (Fish fish in fishBag) fish.SlowDown();
        yield return new WaitForSeconds(iceFishEffect_freezeSecond);
        foreach (Fish fish in fishBag) fish.NormalSpeed();
        CameraController.Instance.PostEffect_IceEnd();
        InIceEffect = false;
    }

    private void ShowBoomFishDieEffect(GameObject boomFish, GameObject net)
    {
        Instantiate(effect_Boom, boomFish.transform.position, net.transform.rotation);
        SoundController.Instance.PlayMusic(Config.Boom);
    }

    private IEnumerator ShowManyFishDieEffect(GameObject manyFish)
    {
        Instantiate(effect_Many, manyFish.transform.position, manyFish.transform.rotation);
        SoundController.Instance.PlayMusic(Config.Many);

        foreach (Fish fish in fishBag) fish.SwimAwayFast();

        int fishCrowedScore = 0;
        int targetRewardScore = ManyFishController.Instance.GetRewardScore();
        while (fishCrowedScore <= targetRewardScore)
        {
            // fish crowed id is from 20-24
            int fishId = Random.Range(20, 25);
            FishData fishData = CsvParser.Instance.FindFishDeepCopy(fishId, false);
            fishData.Score *= ManyFishController.Instance.FishScoreTimes;
            Fish modelFish = SpawnFish(fishData);

            fishCrowedScore += (modelFish.GetFishCrowedScore() * ManyFishController.Instance.FishScoreTimes);

            yield return new WaitForSeconds(0.8f);
        }
    }

    private void ShowOctopusDieEffect()
    {
        UIController.Instance.OctopusAttakEnd();
    }

    public void ShowOctopusSpawnEffect()
    {
        foreach (Fish fish in fishBag) fish.SwimAwayFast();
    }

}
