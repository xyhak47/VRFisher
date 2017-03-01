using UnityEngine;
using System.Collections;

public class nightShiftFade : MonoBehaviour {

    private SpriteRenderer sprite;
    private TextMesh versionText;
    private bool fadeOn = false;
    // Use this for initialization
    void Start () {

        float _z = IVRCameraController._instance.originalCamera.WorldToViewportPoint(transform.position).z;
        transform.position = IVRCameraController._instance.originalCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _z));
        sprite = this.GetComponent<SpriteRenderer>();
        versionText = this.GetComponentInChildren<TextMesh>();
        StartCoroutine(fade());
	}
	
	// Update is called once per frame
	void Update () {
        if (fadeOn)
        {
            sprite.color = new Color(1f, 1f, 1f, sprite.color.a - Time.deltaTime * 2.0f);
            versionText.color = new Color(versionText.color.r, versionText.color.g, versionText.color.b, versionText.color.a - Time.deltaTime * 2.0f);
            if (sprite.color.a < 0.01f)
                Destroy(gameObject);
        }

    }

    IEnumerator fade()
    {
        yield return new WaitForSeconds(2);
        fadeOn = true;
    }
}
