using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static public UIController Instance = null;
    UIController()
    {
        Instance = this;
    }

    [SerializeField]
    private Text gunScore;

    [SerializeField]
    private Text bulluetCost;

    [SerializeField]
    private Image bulletCostImage;

    [SerializeField]
    private Sprite[] bulletImages;

    [SerializeField]
    private Text message;

    [SerializeField]
    private GameObject rebornUI;

    [SerializeField]
    private Text rebornText;

    [SerializeField]
    private GameObject resultUI;

    [SerializeField]
    private Text resultScore;

    [SerializeField]
    private Text config;

    public void SetGunScore(int score)
    {
        gunScore.text = score.ToString();
    }

    public void SetBulletCost(BulletData bullet)
    {
        bulluetCost.text = bullet.Cost.ToString();
        bulletCostImage.sprite = bulletImages[bullet.Id];
    }

    public void SetConfig(string c)
    {
        config.text = c;
    }

    public void Init()
    {
        BulletData bullet = CsvParser.Instance.List_BulletData[0];
        SetBulletCost(bullet);

        SetGunScore(GameController.Instance.GunScore);
    }

    public IEnumerator ShowMsg(string msg)
    {
        message.transform.parent.gameObject.active = true;
        message.text = msg;
        yield return new WaitForSeconds(3);
        message.transform.parent.gameObject.active = false;
    }

    public void ShowRebornUI(bool b)
    {
        rebornUI.gameObject.active = b;

        if(b)
        {
            rebornText.text = "是否尝试花费" + MyServerManager.GamePrice + "积分用于复活？";
            SoundController.Instance.PlayMusic(Config.Fail);
        }
    }

    public IEnumerator ShowGameResult(float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        resultUI.gameObject.active = true;
        resultScore.text = GameController.Instance.GunScore.ToString();


        // sound
        SoundController.Instance.PlayMusic(Config.GameOver);

        yield return new WaitForSeconds(GameController.Instance.EndGameWaitSeconds);
        GameController.Instance.EndGame();
    }
}
