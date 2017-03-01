using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime;

	void Start ()
    {
        Destroy(gameObject, lifetime);
    }
}
