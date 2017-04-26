using UnityEngine;
using System.Collections;
using Custom;

public class PumpingRateController : MonoBehaviour
{
    static public PumpingRateController Instance = null;
    PumpingRateController()
    {
        Instance = this;
    }

    Timer timer = new Timer();

    private float PumpingRateDeltaSec = 5.0f;

    [System.NonSerialized]
    public float PumpingRate = 0.2f;

    void Start()
    {
        timer.tick += OnTimerEvent;
        timer.ResetTriggerTime(PumpingRateDeltaSec);
        timer.Restart();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);
    }

    private void OnTimerEvent(Timer that)
    {
        ResetPumpingRate();
        timer.Restart();
    }

    void OnDestroy()
    {
        timer.Stop();
    }

    private void ResetPumpingRate()
    {
        //PumpingRate = 0.2f * GameController.Instance.GunScore / 
        //    (GameController.Instance.GameScore + (NetRequestManager.Instance.RebornTime - 1) * GameController.Instance.PriceTimes);

        float value = GameController.Instance.GunScore / (float)(GameController.Instance.GameScore + (NetRequestManager.Instance.RebornTime - 1) * GameController.Instance.PriceTimes);
        value = value - 1;

        PumpingRate = 0.2f * Mathf.Pow(2, value);
    }
}
