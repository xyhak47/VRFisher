using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    static public GunController Instance = null;
    GunController()
    {
        Instance = this;
    }

    public Transform bulletPos;
    public Transform gunFireEffectPos;
    public GameObject[] bulletPrefabs;
    public GameObject[] netPrefabs;
    public GameObject fishScorePrefab;
    public GameObject gunFireEffectPrefab;

    public GameObject[] guns;
    private GameObject gun;

    public GameObject fishKilledEffect;
    public GameObject fishKilledEffectBoss;

    private int bulletIndex = 0;

    private float lastTime = 0;
    public float shootDeltaTime = 0.2f;

    [System.NonSerialized]
    public bool IsShootQuit = false;

    void Update()
    {
        if(GameController.Instance.Pause)
        {
            return;
        }

        // if (CanShoot() && Input.GetKey(KeyCode.Alpha2))
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBullet();
        }
    }

    private bool CanShoot()
    {
        if(Time.time - lastTime >= shootDeltaTime)
        {
            lastTime = Time.time;
            return true;
        }

        return false;
    }

    private void Shoot()
    {
        BulletData bullet = CsvParser.Instance.List_BulletData[bulletIndex];

        if (bullet.Cost > GameController.Instance.GunScore)
        {
            if(bulletIndex > 0) // limit to second last  type bullet
            {
                bulletIndex--;
                bulletIndex = Mathf.Clamp(bulletIndex, 0, CsvParser.Instance.List_BulletData.Count - 1);
                ChangeBullet();
                return;
            }
        }


        // animation
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        // bullet
        Instantiate(bulletPrefabs[bulletIndex], bulletPos.position, bulletPos.rotation);
        if(!IsShootQuit)
        {
            GameController.Instance.GunScore -= bullet.Cost;
            UIController.Instance.UpdateScore();
        }


        // effect
        GameObject effect = Instantiate(gunFireEffectPrefab, gunFireEffectPos.position, Quaternion.identity) as GameObject;
        effect.transform.parent = gunFireEffectPos;

        // sound
        SoundController.Instance.PlayMusic(Config.Shoot + bullet.Id);

        GameController.Instance.CheckEndGame();
    }

    private void SelectBullet()
    {
        bulletIndex++;
        bulletIndex = bulletIndex > CsvParser.Instance.List_BulletData.Count - 1 ? 0 : bulletIndex;
        ChangeBullet();
    }

    public void HitFish(GameObject bullet, BulletData bulletData)
    {
        GameObject net = Instantiate(netPrefabs[bulletData.Id], bullet.transform.position, bullet.transform.rotation) as GameObject;
        net.GetComponent<Net>().damage = bulletData.NetDamage;
    }

    public void HitBox(GameObject box)
    {
        box.GetComponent<Box>().PickedUp();
    }

    private void ChangeBullet()
    {
        // bulluet
        BulletData bullet = CsvParser.Instance.List_BulletData[bulletIndex];
        UIController.Instance.SetBulletCost(bullet);

        // gun
        foreach (GameObject g in guns) g.active = false;
        gun = guns[bulletIndex];
        gun.active = true;

        SoundController.Instance.PlayMusic(Config.ChangeBullet);
    }

    public void ResetBullet()
    {
        bulletIndex = 0;
        ChangeBullet();
    }

    public void KillFish(Fish fish, GameObject net)
    {
        WaveController.Instance.KillFish(fish, net);

        // scoreUI
        GameController.Instance.FishKilledScore += fish.fishData.Score;
        GameController.Instance.GunScore += fish.fishData.Score;
        UIController.Instance.UpdateScore();

        GameObject fishscore = Instantiate(fishScorePrefab) as GameObject;
        fishscore.transform.position = fish.transform.position + new Vector3(0, 0.8f, 0);
        fishscore.transform.rotation = net.transform.rotation;
        fishscore.GetComponentInChildren<Text>().text = "+" + fish.GetComponent<Fish>().fishData.Score;
       
        // effect
        if (fish.fishData.IsBoss)
        {
            Instantiate(fishKilledEffectBoss, fish.transform.position, net.transform.rotation);
        }
        else
        {
            Instantiate(fishKilledEffect, fish.transform.position, net.transform.rotation);
        }

        fish.Die();
    }

    public void GunGetHit()
    {
        gun.GetComponent<Gun>().Animation_GetHit();        
    }
}
