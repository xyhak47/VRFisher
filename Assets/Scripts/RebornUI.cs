using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RebornUI : MonoBehaviour
{
    public Text counting;
    public Text msg;
    public int originCountNum;
    public GameObject content;

    private Coroutine countingCoroutine;

    private int countingNum;

    public void Showcontent()
    {
        countingNum = originCountNum;
        counting.text = countingNum.ToString();
        content.gameObject.active = true;
    }

    void OnDisable()
    {
        RebornController.Instance.animationCompleted = false;
        StopCoroutine(countingCoroutine);
    }

    public void Counting()
    {
        countingCoroutine = StartCoroutine(startCounting());

        RebornController.Instance.animationCompleted = true;
    }

    private IEnumerator startCounting()
    {
        if (countingNum == 0)
        {
            RebornController.Instance.CancleReborn();
            yield return null;
        }

        UpdateCounting();
        yield return new WaitForSeconds(1);
        StartCoroutine(startCounting());
    }

    void Awake()
    {
        content.gameObject.active = false;
        countingNum = originCountNum;
    }

    void OnEnable()
    {
        GetComponent<Animator>().SetTrigger("play");
    }

    public void WriteMsg(string m)
    {
        msg.text = m;
    }

    private void UpdateCounting()
    {
        counting.text = countingNum.ToString();
        countingNum--;
    }
}

