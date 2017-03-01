using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    private Vector3 speed = Vector3.zero;


    void OnTriggerEnter(Collider other)
    {
        CoinCollector.Instance.Collect(this);
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, 
            CoinCollector.Instance.transform.position, 
            ref speed, 
            GameController.Instance.CoinCollectDuration);
    }
}
