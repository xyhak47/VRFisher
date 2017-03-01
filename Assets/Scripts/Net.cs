using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Net : MonoBehaviour
{
    [HideInInspector]
    public int damage = 0;

    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        //GetComponent<Animator>().SetTrigger("open");
        Invoke("AfterOpenNet", 0.2f);
    }

    void AfterOpenNet()
    {
        GetComponent<BoxCollider>().enabled = true;
        SoundController.Instance.PlayMusic(Config.Paopao);
    }

    void OnTriggerEnter(Collider other)
    {
        Fish fish = other.gameObject.GetComponent<Fish>();
        Assert.IsTrue(fish != null, "net catch fish null!");

        if (CatchFish(fish))
        {
            fish.Die();
            GunController.Instance.KillFish(fish, this.gameObject);
        }
        else
        {
            fish.Miss();
        }
    }

    private bool CatchFish(Fish fish)
    {
        /*
        value1:  当前炮弹(渔网)威力 *（1 -（抽水率））/ 鱼体型  抽水率暂定为10 %，这数值设为变量，以后会动态变化
        value2:  0.0 - 1.0随机
        判断： value2 <= value1 ==> kill  else not kill
        */

        float value1 = damage * (1 - GameController.Instance.PumpingRate) / fish.fishData.Size;
        float value2 = Random.Range(0.0f, 1.0f);

        if(value2 <= value1)
        {
            return true;
        }

        return false;
    }
}
