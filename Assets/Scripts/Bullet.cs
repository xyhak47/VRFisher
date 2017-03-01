using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 2.0f;
    public BulletData bulletData;
    public int dataId = 0;

	void Start ()
    {
        // attach data
        bulletData = CsvParser.Instance.FindBullet(dataId);

        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        Destroy(gameObject,5);
    }


    void Update()
    {
        //if(transform.position.z > 20)
        //{
        //    Destroy(gameObject);
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        GunController.Instance.HitFish(gameObject, bulletData);
        Destroy(gameObject);
    }
}
