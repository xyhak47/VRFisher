using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Fish fish = other.gameObject.GetComponent<Fish>();

        // boom's position is the same with boomfish, and rotation is the same with the net that hit the boomfish
        // so we can use this.gameObject as net
        if(!fish.fishData.IsBoss)
        {
            GunController.Instance.KillFish(fish, this.gameObject);
        }
    }

    void Start()
    {
        Invoke("DisableTrigger", 0.2f);
    }

    void DisableTrigger()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
}
