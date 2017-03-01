using System.Collections.Generic;
using UnityEngine;

public class CsvParser : MonoBehaviour
{
    /****************************** test *******************************/
    static public CsvParser Instance = null;
    CsvParser()
    {
        Instance = this;
    }

    /***************************** Table Item ****************************/
    public TextAsset CsvItem;
    private List<FishData> List_FishData = new List<FishData>();
    private List<FishData> List_FishData_Boss = new List<FishData>();

    private List<PathData> List_PathData = new List<PathData>();

    public List<BulletData> List_BulletData = new List<BulletData>();

    public List<Wave> List_Wave = new List<Wave>();



    public void ParseDataFromCsv()
    {
        string[] Items = CsvItem.text.Replace("\r", "").Split('\n');

        for (int i = 0; i < Items.Length; i++)
        {
            string EachLine = Items[i];
            string[] DataArr = EachLine.Split(',');

            if (DataArr[0] == Config.FISH)
            {
                FishData New = new FishData(DataArr);

                if (New.IsBoss)
                {
                    List_FishData_Boss.Add(New);
                }
                else
                {
                    List_FishData.Add(New);
                }
            }

            if (DataArr[0] == Config.PATH)
            {
                PathData New = new PathData(DataArr);
                List_PathData.Add(New);
            }

            if (DataArr[0] == Config.BULLET)
            {
                BulletData New = new BulletData(DataArr);
                List_BulletData.Add(New);
            }

            if (DataArr[0] == Config.WAVE)
            {
                Wave New = new Wave(DataArr);
                List_Wave.Add(New);
            }
        }
    }

    public FishData FindFish(int fishId, bool isboss)
    {
        return isboss == false ? List_FishData.Find(it => it.Id == fishId) : List_FishData_Boss.Find(it => it.Id == fishId);
    }

    public BulletData FindBullet(int bulletId)
    {
        return List_BulletData.Find(it => it.Id == bulletId);
    }

    public List<FishData> GetRandomFishFromCurrentWave()
    {
        List<FishData> list = new List<FishData>();

        if (!WaveController.Instance.CurrentWave.IsBoss)
        {
            int TotalScore = 0;
            while (TotalScore < GameController.Instance.GunScore)
            {
                FishData fishData = List_FishData[Random.Range(0, List_FishData.Count - 1)];
                list.Add(fishData);

                TotalScore += fishData.Score;
            }
        }
        else
        {
            FishData randomBoss = List_FishData_Boss[Random.Range(0, List_FishData_Boss.Count - 1)];
            list.Add(randomBoss);
        }

        return list;
    }

    public PathData GetRandomPath()
    {
        PathData randomPath = List_PathData[Random.Range(0, List_PathData.Count - 1)];

        return randomPath;
    }
}




/***** Class *************************************************************/
public class Base
{
    public int Id { get; set; }
}

public class FishData : Base
{
    public int Size { get; private set; }
    public int Score { get; private set; }
    public float Speed { get; private set; }
    public bool IsBoss { get; private set; }

    public FishData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]);
        Size = int.Parse(DataArr[2]);
        Score = int.Parse(DataArr[3]);
        Speed = float.Parse(DataArr[4]);
        IsBoss = int.Parse(DataArr[5]) == 1;
    }
}

public class BulletData : Base
{
    public int Level { get; private set; }
    public int Cost { get; private set; }
    public int NetDamage { get; private set; }

    public BulletData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]);
        Level = int.Parse(DataArr[2]);
        Cost = int.Parse(DataArr[3]);
        NetDamage = int.Parse(DataArr[4]);
    }
}

public class Wave : Base
{
    public float Rate { get; private set; }
    public float Delay { get; private set; }
    public bool IsBoss { get; private set; }
    public int WaveDelay { get; private set; }


    public Wave(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]); // wave id
        Rate = float.Parse(DataArr[2]);
        Delay = float.Parse(DataArr[3]);
        IsBoss = int.Parse(DataArr[4]) == 1;
        WaveDelay = int.Parse(DataArr[5]);
    }
}

public class PathData : Base
{
    public PathData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]); 
    }
}

