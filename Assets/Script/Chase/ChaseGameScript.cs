using UnityEngine;

public class ChaseGameScript : MonoBehaviour {

    public GameObject targetArrow;
    public PlayerRunScript runScript;
    public RunnerScript runner;
    public TextMesh finishMessage;

    private static readonly int GAME_OVER_COUNT = 3;
    private static readonly int GAME_STATUS_WAIT = 0;
    private static readonly int GAME_STATUS_PLAY = 1;
    private static readonly int GAME_STATUS_OVER = 2;

    System.DateTime startRunTime;
    int gameStatus = GAME_STATUS_WAIT;
    public int gameOverCnt = 0;
    int totalStep;
    
    public void Update()
    {
        // show target arrow
        if (gameOverCnt != 0)
        {
            targetArrow.SetActive(true);
            Vector3 targetDirection = runner.gameObject.transform.position - transform.position;
            Quaternion targetQuaternion = Quaternion.identity;
            float t = -Mathf.Atan2(targetDirection.z, targetDirection.x) * Mathf.Rad2Deg + 90;
            targetQuaternion.eulerAngles = new Vector3(0, t, 0);
            targetArrow.transform.rotation = targetQuaternion;
        }
        else
        {
            targetArrow.SetActive(false);
        }

        if (gameStatus == GAME_STATUS_WAIT && runScript.isRunning())
        {
                startRunTime = System.DateTime.Now;
                gameStatus = GAME_STATUS_PLAY;
                runner.startRun();
        }
    }


    public void checkGameOver(Transform runnerPoint)
    {
        if (gameStatus == GAME_STATUS_OVER)
        {
            return;
        }
        GameObject[] points = GameObject.FindGameObjectsWithTag("Rotate");
        GameObject nearestPoint = null;
        float minDistance = float.MaxValue;
        foreach (GameObject point in points)
        {
            float distance = Vector3.Distance(transform.position, point.transform.position);
            if (distance < minDistance)
            {
                nearestPoint = point;
                minDistance = distance;
            }
        }
        RunPointScript runPoint = nearestPoint.GetComponent<RunPointScript>();
        if (runPoint.isLink(runnerPoint))
        {
            gameOverCnt = 0;
            return;
        }
        gameOverCnt++;
        if (gameOverCnt >= GAME_OVER_COUNT)
        {
            gameOver();
        }
    }

    public void gameOver()
    {
        if (gameStatus == GAME_STATUS_OVER)
        {
            return;
        }
        gameStatus = GAME_STATUS_OVER;
        System.TimeSpan duration = System.DateTime.Now - startRunTime;
        totalStep = runScript.getTotalStep();
        finishMessage.text = "追丟了!\n跑了" + totalStep + "步\n" + duration.Minutes + "分" + duration.Seconds + "秒";
    }
}
