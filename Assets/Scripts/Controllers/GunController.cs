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

    public GameObject gun;

    public GameObject fishKilledEffect;
    public GameObject fishKilledEffectBoss;

    private int bulletIndex = 0;

    private float lastTime = 0;
    public float shootDeltaTime = 0.2f;

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
            ChangeBullet();
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
        // animation
        gun.GetComponent<Animator>().SetTrigger("shoot");

        // bullet
        Instantiate(bulletPrefabs[bulletIndex], bulletPos.position, bulletPos.rotation);
        BulletData bullet = CsvParser.Instance.List_BulletData[bulletIndex];
        GameController.Instance.GunScore -= bullet.Cost;
        UIController.Instance.SetGunScore(GameController.Instance.GunScore);

        // effect
        GameObject effect = Instantiate(gunFireEffectPrefab, gunFireEffectPos.position, Quaternion.identity) as GameObject;
        effect.transform.parent = gunFireEffectPos;

        // sound
        SoundController.Instance.PlayMusic(Config.Shoot);

        if (GameController.Instance.GunScore <= 0)
        {
            GameController.Instance.GunScore = 0;
            UIController.Instance.SetGunScore(GameController.Instance.GunScore);
            GameController.Instance.ShowReborn();
        }
    }

    public void HitFish(GameObject bullet, BulletData bulletData)
    {
        GameObject net = Instantiate(netPrefabs[bulletData.Id], bullet.transform.position, bullet.transform.rotation) as GameObject;
        net.GetComponent<Net>().damage = bulletData.NetDamage;
    }

    private void ChangeBullet()
    {
        bulletIndex++;
        bulletIndex = bulletIndex > CsvParser.Instance.List_BulletData.Count - 1 ? 0 : bulletIndex;
        BulletData bullet = CsvParser.Instance.List_BulletData[bulletIndex];
        UIController.Instance.SetBulletCost(bullet);

        SoundController.Instance.PlayMusic(Config.ChangeBullet);
    }

    public void KillFish(Fish fish, GameObject net)
    {
        GameController.Instance.GunScore += fish.fishData.Score;
        UIController.Instance.SetGunScore(GameController.Instance.GunScore);

        GameObject fishscore = Instantiate(fishScorePrefab) as GameObject;
        fishscore.transform.position = fish.transform.position + new Vector3(0, 0.8f, 0);
        fishscore.transform.rotation = net.transform.rotation;
        fishscore.GetComponentInChildren<Text>().text = "+" + fish.GetComponent<Fish>().fishData.Score;

        SoundController.Instance.PlayMusic(Config.FishDie);

        // effect
        if(fish.fishData.IsBoss)
        {
            Instantiate(fishKilledEffectBoss, fish.transform.position, net.transform.rotation);
        }
        else
        {
            Instantiate(fishKilledEffect, fish.transform.position, net.transform.rotation);
        }
    }
}
