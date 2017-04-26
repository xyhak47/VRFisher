using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Fish : MonoBehaviour
{
    [System.NonSerialized]
    public FishData fishData;

    public bool isWithCrowed = false;
    public Vector3[] fishCrowedoffsets;

    public void AttachFishData(FishData data)
    {
        fishData = data;

        // check boss
        if (fishData.IsBoss)  StartCoroutine(UIController.Instance.ShowBoss());
        
        // check special fish
        if(!IsSpecialFish())  WaveController.Instance.fishBag.Add(this);
    }

    // Die by hand
    public void Die()
    {
        if (!IsSpecialFish()) WaveController.Instance.fishBag.Remove(this);

        // destroy parent, parent is its path
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void Miss()
    {
        StartCoroutine(Blink(0.1f, 6));
    }

    // Die by path end, which is called by parent object's OnDestroy() function
    void OnDestroy()
    {
        if (!IsSpecialFish()) WaveController.Instance.fishBag.Remove(this);
    }

    IEnumerator Blink(float delayBetweenBlinks, int numberOfBlinks)
    {
        var counter = 0;
        Color blinkColor = WaveController.Instance.fishBlinkColor;
        while (counter <= numberOfBlinks)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BlinkColor", blinkColor);
            counter++;
            blinkColor.a = blinkColor.a == 1f ? 0f : 1f;
            yield return new WaitForSeconds(delayBetweenBlinks);
        }

        // revert to our standard sprite color
        blinkColor.a = 0f;
        GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BlinkColor", blinkColor);
    }


    // random animation
    private Animator pathAnimator;
    private Animation fishAnimation;

    private float _rand1;
    private float _rand2;

    private float speedRate = 1.0f;

    void Start()
    {
        // check octopus, if not allowed to spawn, directly die
        if (IsFunctionFish(FunctionFish.Octopus))
        {
            if (OctopusController.Instance.SpawnAllowed)
            {
                OctopusController.Instance.SpawnAllowed = false;
            }
            else
            {
                Die();
            }
        }

        // check manyfish, if not allowed to spawn, directly die
        if (IsFunctionFish(FunctionFish.ManyFish))
        {
            if(ManyFishController.Instance.AllowToSpawn())
            {
                ManyFishController.Instance.AlreadyExist = true;
                ManyFishController.Instance.SpawnOnce();
            }
            else
            {
                Die();
            }
        }


        if (!fishData.IsBoss)
        {
            pathAnimator = GetComponentInParent<Animator>();

            fishAnimation = GetComponent<Animation>();
            if (fishAnimation == null)
            {
                fishAnimation = GetComponentInChildren<Animation>();
            }

            _rand1 = Random.Range(0.5f, 1.5f);
            _rand2 = Random.Range(0.5f, 1.5f);
        }

        // normal fish and boss fish are both in fishbag, they can accept ice effect
        if(!IsSpecialFish() && WaveController.Instance.InIceEffect)
        {
            SlowDown();
        }
    }

    void Update()
    {
        // we Temporarily stop perlinnoise effect on boss and octopus, 
        // because boss's animation won't allow and octopus has its special path animaion
        if (!fishData.IsBoss && !fishData.IsOctopus())
        {
            float speed = Mathf.PerlinNoise(Time.time * _rand1, Time.time * _rand2) + 1f;

            if(pathAnimator)                                pathAnimator.speed = speed * speedRate;
            if(fishAnimation && fishAnimation["Motion"])    fishAnimation["Motion"].speed = speed * speedRate;
        }
    }

    public void SlowDown()
    {
        speedRate = 0.2f;
        Freeze();
    }

    public void NormalSpeed()
    {
        speedRate = 1.0f;
        GetRidOfFreeze();
    }

    public void SwimAwayFast()
    {
        speedRate = 4.0f;
    }

    private void Freeze()
    {
        if(GetComponentInChildren<SkinnedMeshRenderer>().material.GetColor("_BodyColor") != null)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", WaveController.Instance.fishFreezeColor);
        }
    }

    private void GetRidOfFreeze()
    {
        if (GetComponentInChildren<SkinnedMeshRenderer>().material.GetColor("_BodyColor") != null)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_BodyColor", Color.white);
        }
    }

    private bool IsSpecialFish()
    {
        return IsFunctionFish(FunctionFish.IceFish) ||
            IsFunctionFish(FunctionFish.ManyFish) ||
            IsFunctionFish(FunctionFish.BoomFish) ||
            IsFunctionFish(FunctionFish.Octopus);
    }

    public enum FunctionFish
    {
        // value == id in data table
        IceFish = 50,
        ManyFish = 51,
        BoomFish = 52,
        Octopus = 53,
    }

    public bool IsFunctionFish(FunctionFish fish)
    {
        return fishData.Id == (int)fish;
    }

    public int GetFishCrowedScore()
    {
        return fishData.Score * (1 + fishCrowedoffsets.Length);
    }
}
