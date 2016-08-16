using UnityEngine;
using System.Collections;

public class RunPointScript : MonoBehaviour {

    public RunPointScript[] nextPoint;
    public RunnerScript runner;

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Runner")
        {
            runner.updateTargetRunPoint(getNextRunPoint());
        }
    }

    public bool isLink(Transform targetPoint)
    {
        if (gameObject.transform == targetPoint)
        {
            return true;
        }
        foreach(RunPointScript next in nextPoint)
        {
            if(next.gameObject.transform == targetPoint)
            {
                return true;
            }
        }
        return false;
    }

    private Transform getNextRunPoint()
    {
        RunPointScript[] tmp = new RunPointScript[nextPoint.Length - 1];
        int j = 0;
        for(int i = 0; i < tmp.Length; i++)
        {
            if(nextPoint[j].gameObject.transform == runner.lastRunPoint)
            {
                j++;
            }
            tmp[i] = nextPoint[j];
            j++;
        }
        int random = Random.Range(0, tmp.Length);
        return tmp[random].gameObject.transform;
    }
}
