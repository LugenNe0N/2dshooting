using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
	// Playerプレハブ
	public GameObject player;
	
	// タイトル
	private GameObject title;

    private GameObject Joystick;

    GameObject Status;
    GameObject restartButton;
    GameObject returnButton;
    GameObject HpGauge;
    GameObject ExpGauge;
    GameObject LevelTitle;
    GameObject Level;
    bool endFlag = false; //ゲームオーバー用
    bool clearFlag = false; //ゲームクリア用


    void Start ()
	{
        //Joystickの設定
        Joystick = GameObject.Find("MobileSingleStickControl");
        Joystick.SetActive(false);
        //Status
        Status = GameObject.Find("Status");
        this.Status.GetComponent<Text>().text = "Tap to START";
        //Restartの表示
        restartButton = GameObject.Find("Restart");
        restartButton.SetActive(false);
        //Titletextの表示
        returnButton = GameObject.Find("Return");
        returnButton.SetActive(false);
        HpGauge = GameObject.Find("HpGauge");
        HpGauge.SetActive(false);
        ExpGauge = GameObject.Find("ExpGauge");
        ExpGauge.SetActive(false);
        LevelTitle = GameObject.Find("LevelTitle");
        LevelTitle.SetActive(false);
        Level = GameObject.Find("Level");
        Level.SetActive(false);
    }

    void OnGUI()
    {
        // ゲーム中ではなく、タッチまたはマウスクリック直後であればtrueを返す。
        if (endFlag == false && IsPlaying() == false && Event.current.type == EventType.MouseDown)
        {
            GameStart();
        }
    }

    void GameStart ()
	{
		// ゲームスタート時に、タイトルを非表示にしてプレイヤーを作成する
        Status.SetActive(false);
        Joystick.SetActive(true);
        HpGauge.SetActive(true);
        ExpGauge.SetActive(true);
        LevelTitle.SetActive(true);
        Level.SetActive(true);
        Instantiate (player, player.transform.position, player.transform.rotation);
	}
	
	public void GameOver ()
	{
		FindObjectOfType<Score> ().Save ();
        // ゲームオーバー時に、タイトルを表示する
        Status.GetComponent<Text>().text = "GAME OVER!!";
        Status.SetActive (true);
        restartButton.SetActive(true);
        returnButton.SetActive(true);
        endFlag = true;
    }

    public bool IsPlaying()
    {
        // ゲーム中かどうかはタイトルの表示/非表示で判断する
        return Status.activeSelf == false && Status.activeSelf == false;
    }
}