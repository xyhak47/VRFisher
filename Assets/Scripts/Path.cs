using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour
{
    private Animator animator;

    public void PathToEnd()
    {
        Destroy(gameObject);
    }



    void Start()
    {
        animator = GetComponent<Animator>();
    }


    // octopus 
    public void BeginOctopusAttak()
    {
        StartCoroutine(OctopusAttak());
    }

    private IEnumerator OctopusAttak()
    {
        animator.speed = 0f;
        if (GetComponentInChildren<Octopus>())
        {
            GetComponentInChildren<Octopus>().BeginAttack();
        }

        UIController.Instance.OctopusAttakBegin();

        yield return new WaitForSeconds(Random.Range(13.0f, 21.0f));

        animator.speed = 1.0f;
        if (GetComponentInChildren<Octopus>())
        {
            GetComponentInChildren<Octopus>().EndAttack();
        }
        UIController.Instance.OctopusAttakEnd();
    }
}
