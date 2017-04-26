using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{

	void Start ()
    {
        NetRequestManager.Instance.CheckVersion("v2.4");
    }



}
