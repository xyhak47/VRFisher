using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;

public class OctopusController : MonoBehaviour
{
    static public OctopusController Instance = null;
    OctopusController()
    {
        Instance = this;
    }

    [System.NonSerialized] // spawn trigger
    public bool SpawnAllowed = false;

    [System.NonSerialized]
    public int OctopusId = 53;

    public GameObject TargetToAttack;

    public int attackScore;

    [SerializeField]
    private float startRandomMinute;

    [SerializeField]
    private float endRandomMinute;

    private Timer timer = new Timer();

    void Start()
    {
        timer.tick += OnTimerEvent;
        ResetTimer();

        // test
        //SpawnAllowed = true;
    }

    void Update()
    {
        timer.Update(Time.deltaTime);
    }

    private void OnTimerEvent(Timer that)
    {
        // open spawn trigger
        SpawnAllowed = true;

        // directly spawn a octopus, id is 53
        WaveController.Instance.SpawnFish(OctopusId);
        WaveController.Instance.ShowOctopusSpawnEffect();

        // repeat to spawn
        ResetTimer();
    }

    public void PlayerGetAttacked()
    {
        if (GameController.Instance.Pause) return;

        //print("PlayerGetAttacked");
        GameController.Instance.GunScore -= attackScore;
        UIController.Instance.UpdateScore();
        GameController.Instance.CheckEndGame();

        UIController.Instance.PlayerGetAttacked();
        GunController.Instance.GunGetHit();
    }

    private float GetRandomSec()
    {
        return 60.0f * Random.Range(startRandomMinute, endRandomMinute);
    }

    private void ResetTimer()
    {
        timer.ResetTriggerTime(GetRandomSec());
        timer.Restart();
    }

    public Vector3 GetAttackTargetRandomPosition()
    {
        Vector3 pos = TargetToAttack.transform.position;
        Vector3 scale = TargetToAttack.transform.localScale;

        float random_x = Random.Range(-scale.x, scale.x);
        float random_y = Random.Range(-scale.y, scale.y);

        return pos + new Vector3(random_x, random_y, 0);
    }
}
