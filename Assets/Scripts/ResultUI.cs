using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Text[] boxNums;

    void OnEnable()
    {
        GetComponent<Animator>().SetTrigger("show");
    }

    public void BeginShow()
    {
        for(int i = 0; i < boxNums.Length; i++)
        {
            boxNums[i].text = "x" + BoxController.Instance.boxs[i];
        }
    }

    public void EndShow()
    {

    }
}
