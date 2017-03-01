using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour
{
    public FishData fishData;
    public int dataId;
    public bool isboss = false;

    public bool isWithCrowed = false;
    public Vector3[] fishCrowedoffsets;

    void Start()
    {
        AttachFishData();
    }

    private void AttachFishData()
    {
        fishData = CsvParser.Instance.FindFish(dataId, isboss);
    }

    public void Die()
    {
        Destroy(gameObject);
        GameController.Instance.KillFish(this);
    }

    public void Miss()
    {
        StartCoroutine(Blink(0.1f, 6));
    }

    IEnumerator Blink(float delayBetweenBlinks, int numberOfBlinks)
    {
        var counter = 0;
        Color blinkColor = GameController.Instance.fishBlinkColor;
        while (counter <= numberOfBlinks)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BlinkColor", blinkColor);
            counter++;
            blinkColor.a = blinkColor.a == 1f ? 0f : 1f;
            yield return new WaitForSeconds(delayBetweenBlinks);
        }

        // revert to our standard sprite color
        blinkColor.a = 0f;
        GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BlinkColor", blinkColor);
    }
}
