using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    private TextAsset CsvItem;
    public TextAsset CsvItem_RaceDog;
    public TextAsset CsvItem_Normal;


    private List<FishData> List_FishData = new List<FishData>();
    private List<FishData> List_FishData_Boss = new List<FishData>();

    private List<PathData> List_PathData = new List<PathData>();
    private List<PathData> List_PathData_Boss = new List<PathData>();
    private List<PathData> List_PathData_Octopus = new List<PathData>();

    public List<BulletData> List_BulletData = new List<BulletData>();

    public List<Wave> List_Wave = new List<Wave>();

    private List<BoxData> List_BoxData = new List<BoxData>();


    void Awake()
    {
        CsvItem = GameController.Instance.IsNormalGame ? CsvItem_Normal : CsvItem_RaceDog;
    }


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

                if (New.IsType(PathData.PathType.NORMAL))
                {
                    List_PathData.Add(New);
                }
                else if (New.IsType(PathData.PathType.BOSS))
                {
                    List_PathData_Boss.Add(New);
                }
                else // if (New.IsType(PathData.PathType.OCTOPUS))
                {
                    List_PathData_Octopus.Add(New);
                }
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

            if (DataArr[0] == Config.BOX)
            {
                BoxData New = new BoxData(DataArr);
                List_BoxData.Add(New);
            }
        }
    }

    public FishData FindFish(int fishId, bool isboss)
    {
        return isboss == false ? List_FishData.Find(it => it.Id == fishId) : List_FishData_Boss.Find(it => it.Id == fishId);
    }

    public FishData FindFishDeepCopy(int fishId, bool isboss)
    {
        return Tool.DeepCopy<FishData>(FindFish(fishId, isboss));
    }

    public BulletData FindBullet(int bulletId)
    {
        return List_BulletData.Find(it => it.Id == bulletId);
    }

    public List<FishData> GetRandomFishFromCurrentWave()
    {
        List<FishData> list = new List<FishData>();

        Wave currentWave = WaveController.Instance.CurrentWave;
        if (!currentWave.IsBoss())
        {
            float TotalScore = 0;

            // v2.4
            //float RateWeight = (NetRequestManager.Instance.RebornTime - 1) * WaveController.Instance.RateWeight;
            //RateWeight = Mathf.Clamp(RateWeight, 1.0f, 1.5f);

            // base is 2
           // float RateWeight = Mathf.Log(MyServerManager.GamePrice / 3.0f + 1.0f, 2.0f);

            float waveLimitedScore = 3000 * currentWave.Rate;
            //float waveLimitedScore = GameController.Instance.GameScore;

            while (TotalScore < waveLimitedScore)
            {
                FishData fishData = List_FishData[Random.Range(0, List_FishData.Count)];
                list.Add(fishData);

                TotalScore += fishData.Score;
            }
        }
        else
        {
            FishData randomBoss = List_FishData_Boss[Random.Range(0, List_FishData_Boss.Count)];
            list.Add(randomBoss);
        }

        return list;
    }

    public PathData GetRandomPath(FishData fishData)
    {
        List<PathData> list = fishData.IsBoss ? List_PathData_Boss : List_PathData;
        list = fishData.IsOctopus() ? List_PathData_Octopus : list;

        PathData randomPath = list[Random.Range(0, list.Count)];

        return randomPath;
    }

    public BoxData GetRandomBox()
    {
        BoxData randomBox = List_BoxData[Random.Range(0, List_BoxData.Count)];

        return randomBox;
    }
}




/***** Class *************************************************************/
[System.Serializable]
public class Base
{
    public int Id { get; set; }
}

[System.Serializable]
public class FishData : Base
{
    public int Size { get; set; }
    public int Score { get; set; }
    public bool IsBoss { get; private set; }
    public bool IsOctopus() { return Id == OctopusController.Instance.OctopusId; }

    public FishData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]);
        Size = int.Parse(DataArr[2]);
        Score = int.Parse(DataArr[3]);
        IsBoss = int.Parse(DataArr[4]) == 1;
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
    public int ContentId { get; private set; }
    public int WaveDelay { get; private set; }

    static int NormalFish = 0;
    static int BossFish = 1;

    public bool IsBoss() { return ContentId == BossFish;  }

    public Wave(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]); // wave id
        Rate = float.Parse(DataArr[2]);
        Delay = float.Parse(DataArr[3]);
        ContentId = int.Parse(DataArr[4]);
        WaveDelay = int.Parse(DataArr[5]);
    }
}

public class PathData : Base
{
    public enum PathType
    {
        NORMAL = 0,
        BOSS = 1,
        OCTOPUS = 2,
    }

    public bool IsType(PathType type)
    {
        return Type == (int)type;
    }

    public int Type { get; private set; }

    public PathData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]);
        Type = int.Parse(DataArr[2]);
    }
}

public class BoxData : Base
{
    public BoxData(string[] DataArr)
    {
        Id = int.Parse(DataArr[1]);
    }
}


public static class Tool
{
    public static T DeepCopy<T>(T other)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, other);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }
}



