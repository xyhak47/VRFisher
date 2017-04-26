using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    public Text playerRank;
    public Text playerScore;

    public List<Text> List_Rank;

    private List<RankInfo> List_RankInfo = null;
    private int RankStoreLimit = int.MaxValue;


    public void UpdateRankingAndShow(int newScore)
    {
        RecoverRankInfoFromLocalRankData();
        AddNewRank(newScore);
        gameObject.active = true;

        print("AddNewRank:" + newScore);
    }

    private void SetRankData(List<RankInfo> List_RankInfo)
    {
        int RanksCount = Mathf.Clamp(List_RankInfo.Count, 0, List_Rank.Count);
        for (int i = 0; i < RanksCount; i++)
        {
            RankInfo Rank = List_RankInfo[i];
            List_Rank[i].text = Rank.Score.ToString() ;
        }

        // Hide the unuse
        for (int i = RanksCount; i < List_Rank.Count; i++)
        {
            List_Rank[i].gameObject.active = false;
        }
    }

    private void SetPlayerRankData(RankInfo info)
    {
        playerRank.text = info.Ranking.ToString();
        playerScore.text = info.Score.ToString();
    }

    private void AddNewRank(int score)
    {
        RankInfo NewRank = new RankInfo(score);
        List_RankInfo.Add(NewRank);
        ResetRankData();

        SetPlayerRankData(NewRank);
    }

    private void ResetRankData()
    {
        // Keep count in RankStoreLimit
        while (List_RankInfo.Count > RankStoreLimit)
        {
            List_RankInfo.RemoveAt(List_RankInfo.Count - 1);
        }

        // Sort
        List_RankInfo.Sort(
            (Left, Right) =>
            {
                // Inverted order
                return Right.Score - Left.Score;
            }
        );

        // Set ranking value
        for (int i = 0; i < List_RankInfo.Count; i++)
        {
            List_RankInfo[i].Ranking = i + 1;
        }

        // Save to local file
        SaveRankData();

        // Update UI
        SetRankData(List_RankInfo);
    }

    private void SaveRankData()
    {
        PlayerPrefs.SetString(Config.RankDataKey, GetRankData());
        PlayerPrefs.Save();
    }

    private string GetRankData()
    {
        string RankData = "";

        foreach (RankInfo Each in List_RankInfo)
        {
            RankData += Each.GetRankData();
        }

        return RankData;
    }

    private void RecoverRankInfoFromLocalRankData()
    {
        // Already exist
        if (List_RankInfo != null && List_RankInfo.Count != 0) return;

        // Recover data from local file
        string LocalRankData = PlayerPrefs.GetString(Config.RankDataKey);
        string[] RankDatas = LocalRankData.Split(';');
        List_RankInfo = new List<RankInfo>(RankDatas.Length);

        for (int i = 0; i < RankDatas.Length; i++)
        {
            string OneRank = RankDatas[i];
            RankInfo Rank = RankInfo.ParseToRankInfo(OneRank);

            if (Rank != null)
            {
                List_RankInfo.Add(Rank);
            }
        }
    }

    public void ClearData()
    {
        PlayerPrefs.SetString(Config.RankDataKey, "");
    }
}


public class RankInfo
{
    public int Ranking;
    public int Score;

    public RankInfo(int score = 0)
    {
        Score = score;
    }

    public static RankInfo ParseToRankInfo(string RankData)
    {
        if (RankData == "")
        {
            return null;
        }

        RankInfo NewRankInfo = new RankInfo();
        string[] Rank = RankData.Split(',');
        NewRankInfo.SetRankData(int.Parse(Rank[0]), int.Parse(Rank[1]));
        return NewRankInfo;
    }

    public void SetRankData(int TempRanking , int TempScore)
    {
        Ranking = TempRanking;
        Score = TempScore;
    }

    public string GetRankData()
    {
        return Ranking.ToString() + "," + Score.ToString() + ";";
    }
}

