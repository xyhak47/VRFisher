using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class WaveController : MonoBehaviour
{
    static public WaveController Instance = null;
    WaveController()
    {
        Instance = this;
    }

    [HideInInspector]
    public Wave CurrentWave = null;

    private bool HasNextWave = true;

    public void BeginWave()
    {
        //print("1");
        StartCoroutine(NextFish());
        //print("2");
    }

    IEnumerator NextFish()
    {
        foreach (FishData fishData in CsvParser.Instance.GetRandomFishFromCurrentWave())
        {
            // print("3");
            SpawnFish(fishData);

            yield return new WaitForSeconds(CurrentWave.Delay * 2);
           // print("4");
        }

        //print("5");
        yield return new WaitForSeconds(CurrentWave.WaveDelay* 2);
        //print("6");

        if(HasNextWave)
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


    public void ResetCurrentWave()
    {
        Assert.IsTrue(CsvParser.Instance.List_Wave.Count != 0, "List_Wave is empty!");

        CurrentWave = CsvParser.Instance.List_Wave[0];
    }

    public void NextWave()
    {
        int next = CurrentWave.Id + 1;
        CurrentWave = CsvParser.Instance.List_Wave[next];
        //next = Mathf.Clamp(next, 0, CsvParser.Instance.List_Wave.Count - 1);
        HasNextWave = next >= CsvParser.Instance.List_Wave.Count - 1 ? false : true;
    }

    private void SpawnFish(FishData fishData)
    {
        GameObject fish = Resources.Load(Config.FishPrefabFolder + fishData.Id) as GameObject;
        fish = Instantiate(fish, Vector3.zero, Quaternion.identity) as GameObject;

        // origin fish
        GameObject path = PathController.Instance.GetRandomPath();
        path = Instantiate(path);
        fish.transform.parent = path.transform;
        fish.transform.localPosition = Vector3.zero;
        fish.transform.localEulerAngles = new Vector3(0, -90, 0);

        // fish crowed
        if (fish.GetComponent<Fish>().isWithCrowed)
        {
            // fish crowed
            foreach (var offsetPos in fish.GetComponent<Fish>().fishCrowedoffsets)
            {
                GameObject subFish = Resources.Load(Config.FishPrefabFolder + fishData.Id) as GameObject;
                subFish = Instantiate(subFish, Vector3.zero, Quaternion.identity) as GameObject;
                subFish.GetComponent<Fish>().isWithCrowed = false;

                subFish.transform.parent = path.transform;
                subFish.transform.localPosition = fish.transform.localPosition + offsetPos;
                subFish.transform.localEulerAngles = new Vector3(0, -90, 0);
            }
        }
    }
}
