using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Net : MonoBehaviour
{
    [System.NonSerialized]
    public int damage = 0;
    private int touchFishNum = 0;

    [SerializeField]
    private bool fromBigGun;

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
        // make sure the net do not working when game is over
        if (GameController.Instance.Pause) return;

        touchFishNum++;

        Fish fish = other.gameObject.GetComponent<Fish>();
        Assert.IsTrue(fish != null, "net catch fish null!");

        if (CatchFish(fish))
        {
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

        //float value1 = damage * (1 - GameController.Instance.PumpingRate) / fish.fishData.Size;
        //float value2 = Random.Range(0.0f, 1.0f);


        if (fromBigGun) // big gun
        {
            damage = damage - 20 * (touchFishNum - 1);
            damage = Mathf.Clamp(damage, 10, int.MaxValue);
        }
        else // small gun
        {
            damage = damage - 10 * (touchFishNum - 1);
            damage = Mathf.Clamp(damage, 5, int.MaxValue);
        }


        float value1 = 0.0f;
        if(damage <= fish.fishData.Size)
        {
            value1 = damage * (1 - PumpingRateController.Instance.PumpingRate) / fish.fishData.Size;
        }
        else //  damage > fish.fishData.Size
        {
            value1 = 1 - fish.fishData.Size * PumpingRateController.Instance.PumpingRate / damage;
        }

        //value1 /= touchFishNum;
        
        // test
        //value1 = -1;

        float value2 = Random.Range(0.0f, 1.0f);


        if (value2 <= value1)
        {
            return true;
        }

        return false;
    }
}
