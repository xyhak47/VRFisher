using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour
{
    public void PathToEnd()
    {
        print(gameObject.name);
        Destroy(gameObject);
    }
}
