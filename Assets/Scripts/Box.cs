using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour
{
    public int id;

    public void PickedUp()
    {
        SoundController.Instance.PlayMusic(Config.GetBox);

        // send msg to server
        BoxController.Instance.CommitBox(this);

        Destroy(gameObject);
    }
}
