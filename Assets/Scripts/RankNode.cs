using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RankNode : MonoBehaviour
{
    public Text Ranking;
    public Text Score;

    public void SetRankData(string ranking, string score)
    {
        Ranking.text = ranking;
        Score.text = score;
    }
}
