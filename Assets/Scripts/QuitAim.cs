using UnityEngine;
using System.Collections;

public class QuitAim : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Quit"))
        {
            //print("QuitAim OnTriggerEnter");
            GunController.Instance.IsShootQuit = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            //print("QuitAim OnTriggerEnter Bullet");
            Destroy(other.gameObject); // 回收子弹

            GameController.Instance.Pause = true;
            StartCoroutine(UIController.Instance.ShowGameResult());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Quit"))
        {
            //print("QuitAim OnTriggerExit");
            GunController.Instance.IsShootQuit = false;
        }
    }
}
