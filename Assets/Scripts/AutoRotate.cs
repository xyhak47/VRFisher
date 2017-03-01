using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
    public  Vector3 rotationDir;

    void Update()
    {
        transform.eulerAngles += new Vector3(rotationDir.x, rotationDir.y, rotationDir.z * Time.deltaTime);
    }
}
