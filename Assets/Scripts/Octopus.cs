using UnityEngine;
using System.Collections;

public class Octopus : MonoBehaviour
{
    public OctopusTalent talent;

    public void BeginAttack()
    {
        talent.PlayAmimation("Attack");
    }

    public void EndAttack()
    {
        talent.PlayAmimation("Swim");
    }

    public void SpeedUp()
    {
        GetComponentInParent<Animator>().speed = 3f;
    }

    public void NormalSpeed()
    {
        GetComponentInParent<Animator>().speed = 1f;
    }
}
