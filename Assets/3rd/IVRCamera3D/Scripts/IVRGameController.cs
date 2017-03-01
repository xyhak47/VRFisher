using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class IVRGameController : MonoBehaviour {

	//dll import function
	[DllImport("libIVR")]
	private static extern void IVR_NotifyGameStart();

	[DllImport("libIVR")]
	private static extern void IVR_NotifyGameStop();

	[DllImport("libIVR")]
	private static extern void IVR_GameSendAliveMessage();

	void Awake()
	{
		IVR_NotifyGameStart();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy()
	{
		IVR_NotifyGameStop();
	}

}
