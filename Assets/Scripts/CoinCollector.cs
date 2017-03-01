using UnityEngine;
using System.Collections;

public class CoinCollector : MonoBehaviour
{
    static public CoinCollector Instance = null;
    CoinCollector()
    {
        Instance = this;
    }

    public void Collect(Coin coin)
    {
        SoundController.Instance.PlayMusic(Config.GetCoin);
    }
}
