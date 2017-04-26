using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private List<Animator> List_BulletAnimator;

    public void Animation_GetHit()
    {
        GetComponent<Animator>().SetTrigger("GetHit");

        foreach(var a in List_BulletAnimator) a.SetTrigger("GetHit");
    }
}
