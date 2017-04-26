using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class OctopusWeapon : MonoBehaviour
{
    public GameObject paticular;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = OctopusController.Instance.GetAttackTargetRandomPosition();
    }

    void OnTriggerEnter(Collider other)
    {
        // OctopusWeapon can hit OctopusWeaponReceiver and Bullet
        if (other.gameObject.layer == LayerMask.NameToLayer("OctopusWeaponReceiver"))
        {
            // attack target
            OctopusController.Instance.PlayerGetAttacked();
        }

        Instantiate(paticular, transform.position, transform.rotation);
        SoundController.Instance.PlayMusic(Config.OctopusWeapon); // explosion

        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 0.6f);
    }
}
