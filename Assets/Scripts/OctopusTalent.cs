using UnityEngine;
using System.Collections;
using Custom;

public class OctopusTalent : MonoBehaviour
{
    public float talentDeltaSec;
    public GameObject weaponPrefab;
    public Transform weaponPos;
    public Octopus parent;

    public void Attack()
    {
        Instantiate(weaponPrefab, weaponPos.position, weaponPos.rotation);
    }

    public void PlayAmimation(string name)
    {
        GetComponent<Animator>().SetTrigger(name);
    }

    public void SpeedUp()
    {
        parent.SpeedUp();
    }

    public void NormalSpeed()
    {
        parent.NormalSpeed();
    }
}
