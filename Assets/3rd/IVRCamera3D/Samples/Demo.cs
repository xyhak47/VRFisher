using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour {

	// Use this for initialization
	void Start () {


    }

    void Awake()
    {
        //调用此方法来连接客户端；可以在某个物体的Awake()方法中来调用此函数
        MyServerManager.GameConnectToServer();
    }
	
	// Update is called once per frame
	void Update () {

        //游戏启动后，调用此方法来向后台发送“已正常开始场景”消息。如按键开始游戏，可在按键后使用该方法
        if (Input.GetKey(KeyCode.Space))
            MyServerManager.StartSuccess();
    }

    void Death()
    {
        //关闭游戏时，调用此方法来向后台发送“场景已结束，请关闭场景”消息
        MyServerManager.EndGame();
    }
}
