using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;

public class BoxController : MonoBehaviour
{
    static public BoxController Instance = null;
    BoxController()
    {
        Instance = this;
    }

    public GameObject effect_Box;

    [System.NonSerialized]
    public int[] boxs = { 0, 0, 0, 0, 0, 0 };

    public void CommitBox(Box box)
    {
        boxs[box.id]++;
        Instantiate(effect_Box, box.transform.position, box.transform.rotation);
        StartCoroutine(TryCommitBox(box.id));
    }

    private IEnumerator TryCommitBox(int type)
    {
        yield return null;
        string msg = "";
        bool succeed = NetRequestManager.Instance.TryBox(ref msg, type);
        StartCoroutine(UIController.Instance.ShowMsg(msg));
    }


    [System.Serializable]
    public class BoxSpawnTime
    {
        [System.NonSerialized]
        public Timer timer = new Timer();

        [Range(0.0f, 180f)]
        public float boxTime;

        public void Update()
        {
            timer.Update(Time.deltaTime);
        }
    }

    public List<BoxSpawnTime> List_BoxSpawnTime;

    void Awake()
    {
        // normal game dont show box
        if (GameController.Instance.IsNormalGame)
        {
            gameObject.active = false;
        }
    }

    void Start()
    {
        foreach (var box in List_BoxSpawnTime)
        {
            box.timer.tick += OnTimerEvent;
            box.timer.ResetTriggerTime(box.boxTime);
            box.timer.Restart();
        }
    }

    void Update()
    {
        foreach (var box in List_BoxSpawnTime)
        {
            box.Update();
        }
    }

    private void OnTimerEvent(Timer that)
    {
        SpawnBox();
        that.Stop();
    }

    private void SpawnBox()
    {
        // box rate = 0.9
        if (Random.Range(0.0f, 1.0f) > 0.9f) return;

        BoxData boxData = CsvParser.Instance.GetRandomBox();

        GameObject box = Resources.Load(Config.BoxPrefabFolder + boxData.Id) as GameObject;
        Instantiate(box);
    }
}
