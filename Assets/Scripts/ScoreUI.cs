using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private GameObject content_raceDog;

    [SerializeField]
    private GameObject content_normal;

    [SerializeField]
    private Text gunScore_raceDog;

    [SerializeField]
    private Text gunScore_normal;

    [SerializeField]
    private Text gainScore_normal;

    [SerializeField]
    private GameObject blinkRedFrame_normal;
    [SerializeField]
    private GameObject blinkRedFrame_raceDog;
    private GameObject blinkRedFrame;

    [SerializeField]
    private Text hurtScore_normal;
    [SerializeField]
    private Text hurtScore_raceDog;
    private Text hurtScore;

    [SerializeField]
    private GameObject attackedTip_normal;
    [SerializeField]
    private GameObject attackedTip_raceDog;
    private GameObject attackedTip;

    void Awake()
    {
        content_normal.gameObject.SetActive(false);
        content_raceDog.gameObject.SetActive(false);
    }

    public void SetScoreUIType(bool isNormal)
    {
        content_normal.gameObject.SetActive(isNormal);
        content_raceDog.gameObject.SetActive(!isNormal);

        blinkRedFrame = isNormal ? blinkRedFrame_normal : blinkRedFrame_raceDog;
        hurtScore = isNormal ? hurtScore_normal : hurtScore_raceDog;
        attackedTip = isNormal ? attackedTip_normal : attackedTip_raceDog;

        hurtScore.text = (-OctopusController.Instance.attackScore).ToString();
    }

    public void UpdateScore(int gunScore, int gainScore)
    {
        gunScore_raceDog.text = gunScore.ToString();
        gunScore_normal.text = gunScore.ToString();

        gainScore_normal.text = gainScore.ToString();
    }

    public void PlayerGetAttacked()
    {
        StartCoroutine(Blink(0.1f, 6));
    }

    IEnumerator Blink(float delayBetweenBlinks, int numberOfBlinks)
    {
        var counter = 0;
        while (counter <= numberOfBlinks)
        {
            hurtScore.gameObject.active = true;
            blinkRedFrame.active = !blinkRedFrame.active;
            counter++;
            yield return new WaitForSeconds(delayBetweenBlinks);
        }

        hurtScore.gameObject.active = false;
        blinkRedFrame.active = false;
    }

    public void OctopusAttakBegin()
    {
        attackedTip.active = true;
    }

    public void OctopusAttakEnd()
    {
        attackedTip.active = false;
    }
}
