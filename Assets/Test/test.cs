using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{
	void Start ()
    {
        MyServerManager.WeChatPayId = "14770217145420115";
        MyServerManager.GamePrice = 5;
	}


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            string msg = "";
            bool result = RebornController.Instance.TryReborn(ref msg);

            print(msg);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RebornController.Instance.RebornTime++;
            print("RebornTime=" + RebornController.Instance.RebornTime);
        }

    }

}
