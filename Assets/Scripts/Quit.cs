using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Quit : MonoBehaviour
{
    public Image red;
    public float deltaSeconds = 5;

    private bool InCounting = false;

    void OnTriggerEnter(Collider other)
    {
        InCounting = true;
    }

    void OnTriggerExit(Collider other)
    {
        Reset();
    }

    void Update()
    {
        if(InCounting)
        {
            red.fillAmount += Time.fixedDeltaTime / deltaSeconds;

            if(red.fillAmount == 1)
            {
                QuitGame();
            }
        }
    }

    private void Reset()
    {
        InCounting = false;
        red.fillAmount = 0;
    }

    private void QuitGame()
    {
        GameController.Instance.EndGame();
    }
}
