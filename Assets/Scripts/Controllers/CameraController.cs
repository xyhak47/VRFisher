using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    static public CameraController Instance = null;
    CameraController()
    {
        Instance = this;
    }

    [System.NonSerialized]
    public Camera left;

    [System.NonSerialized]
    public Camera right;

    public void PostEffect_IceStart()
    {
        left.GetComponent<IVRLensCorrection>().material.SetFloat("_IceTrigger", 1.0f);
        right.GetComponent<IVRLensCorrection>().material.SetFloat("_IceTrigger", 1.0f);
    }

    public void PostEffect_IceEnd()
    {
        left.GetComponent<IVRLensCorrection>().material.SetFloat("_IceTrigger", 0.0f);
        right.GetComponent<IVRLensCorrection>().material.SetFloat("_IceTrigger", 0.0f);
    }
}
