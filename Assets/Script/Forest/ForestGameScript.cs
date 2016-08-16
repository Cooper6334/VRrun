using UnityEngine;
using System.Collections;

public class ForestGameScript : MonoBehaviour
{
    public AudioSource gameoverSound;
    public PlayerRunScript runScript;
    public TitanFollowScript enemy;
    public TextMesh finishMessage;

    private static readonly int GAME_STATUS_WAIT = 0;
    private static readonly int GAME_STATUS_PLAY = 1;
    private static readonly int GAME_STATUS_OVER = 2;

    System.DateTime startRunTime;
    int totalStep;
    int gameStatus = GAME_STATUS_WAIT;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (gameStatus == GAME_STATUS_WAIT && runScript.isRunning())
        {
            startRunTime = System.DateTime.Now;
            gameStatus = GAME_STATUS_PLAY;
            enemy.startRun();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            gameOver();
            enemy.gameOver();
        }
    }
    private void gameOver()
    {
        if (gameStatus == GAME_STATUS_OVER)
        {
            return;
        }
        gameStatus = GAME_STATUS_OVER;
        gameoverSound.PlayDelayed(0.6f);
        System.TimeSpan duration = System.DateTime.Now - startRunTime;
        totalStep = runScript.getTotalStep();
        enemy.gameOver();
        finishMessage.text = "你跑了\n" + totalStep + "步\n" + duration.Minutes + ":" + duration.Seconds;
    }
}
