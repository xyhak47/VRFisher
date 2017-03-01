using UnityEngine;
using System.Collections;

public class copyrightControl : MonoBehaviour {

    private SpriteRenderer sprite;
    private Camera myCamera;

    // Use this for initialization
    void Start () {
        sprite = this.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        InvokeRepeating("changeLogo", 10f, 10f);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void changeLogo()
    {
        bool dice = Random.value < 0.5 ? false : true;
        float _x = dice ? Mathf.Abs(Mathf.RoundToInt(Random.value) - 0.05f) : Random.Range(0.05f, 0.95f);
        float _y = dice ? Random.Range(0.05f, 0.95f) : Mathf.Abs(Mathf.RoundToInt(Random.value) - 0.05f);
        float _z = IVRCameraController._instance.originalCamera.WorldToViewportPoint(transform.position).z;

        this.transform.position = IVRCameraController._instance.originalCamera.ViewportToWorldPoint(new Vector3(_x, _y, _z));
        StartCoroutine(showLogo());
    }

    IEnumerator showLogo()
    {
        sprite.enabled = true;
        yield return new WaitForSeconds(4);
        sprite.enabled = false;
    }
}
