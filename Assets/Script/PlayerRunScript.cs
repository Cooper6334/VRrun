using UnityEngine;
using System.Collections;

public class PlayerRunScript : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public GameObject head;
    public SteamVR_TrackedObject leftController;
    public SteamVR_TrackedObject rightController;

    private static readonly float MIN_SPEED = 1f;
    private static readonly float MAX_SPEED = 3f;
    private static readonly float ADD_SPEED = 0.5f;
    private static readonly float MINUS_SPEED = 0.3f;
    private static readonly float HEAD_STEP_MIN_VELOCITY = 0.2f;
    private static readonly float HEAD_JUMP_MIN_VELOCITY = 0.8f;
    private static readonly float HEAD_DOWN_MIN_VELOCITY = -1f;
    private static readonly float HEAD_DOWN_MIN_HEIGHT = 1.2f;
    private static readonly int CROSS_TRAP_DURATION_MILLI = 500;
    private static readonly int STEP_MAX_DURATION_MILLI = 1000;
    private static readonly int STEP_MINUS_DURATION_MILLI = 100;
    private static readonly int START_MOVE_STEP = 3;
    private static readonly int RUN_STATUS_NORMAL = 0;
    private static readonly int RUN_STATUS_BLOCK_JUMP = 1;
    private static readonly int RUN_STATUS_BLOCK_DOWN = 2;
    private static readonly int STEP_DIRECTION_NONE = 0;
    private static readonly int STEP_DIRECTION_UP = 1;
    private static readonly int STEP_DIRECTION_DOWN = 2;

    System.DateTime lastStepTime;
    System.DateTime lastJumpTime;
    System.DateTime lastDownTime;
    Vector3 lastHeadPosition;
    Vector3 runDirection;
    GameObject lastHitTrap;
    float speed = 0;
    int lastStepDirection = STEP_DIRECTION_NONE;
    int runStatus = RUN_STATUS_NORMAL;
    int stepMoveCnt;
    bool isHeadCrossJump;
    bool isHeadCrossDown;
    int headStepDirection;
    int totalStep;
    
    // Update is called once per frame
    void Update()
    {
        updateHeadStatus();
        // check cross trap
        if (runStatus == RUN_STATUS_BLOCK_JUMP && isHeadCrossJump)
        {
            StartCoroutine(ignorCollider(lastHitTrap.GetComponent<Collider>()));
            runStatus = RUN_STATUS_NORMAL;
            if (speed < MAX_SPEED)
            {
                speed = MAX_SPEED;
            }
            myRigidbody.velocity = head.transform.forward * speed;
        }
        if (runStatus == RUN_STATUS_BLOCK_DOWN && isHeadCrossDown)
        {
            StartCoroutine(ignorCollider(lastHitTrap.GetComponent<Collider>()));
            runStatus = RUN_STATUS_NORMAL;
            if (speed < MAX_SPEED)
            {
                speed = MAX_SPEED;
            }
            myRigidbody.velocity = head.transform.forward * speed;
        }
        // if cross check failed, block moving
        if (runStatus != RUN_STATUS_NORMAL)
        {
            speed = 0;
            myRigidbody.velocity = head.transform.forward * speed;
            return;
        }
        // check step and update speed
        System.TimeSpan duration = System.DateTime.Now - lastStepTime;
        if (checkStep(duration))
        {
            lastStepTime = System.DateTime.Now;
            speed += ADD_SPEED;
            if (speed > MAX_SPEED)
            {
                speed = MAX_SPEED;
            }else if(speed < MIN_SPEED)
            {
                speed = MIN_SPEED;
            }
        }
        else  if (duration.TotalMilliseconds > STEP_MINUS_DURATION_MILLI)
        {
            speed -= MINUS_SPEED;
            if (speed <= 0)
            {
                speed = 0;
            }
            else
            {
                lastStepTime = System.DateTime.Now;
            }
        }
        // update direction
        bool leftTrigger = false;
        bool rightTrigger = false;
        try
        {
            SteamVR_Controller.Device left = SteamVR_Controller.Input((int)leftController.index);
            leftTrigger = left.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        }
        catch (System.Exception e)
        {

        }
        try
        {
            SteamVR_Controller.Device right = SteamVR_Controller.Input((int)rightController.index);
            rightTrigger = right.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        }
        catch (System.Exception e)
        {

        }

        bool isTrigger = leftTrigger || rightTrigger;
        if (!isTrigger)
        {
            runDirection = head.transform.forward;
        }
        myRigidbody.velocity = new Vector3(runDirection.x * speed, 0, runDirection.z * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Jump")
        {
            print("player hit Jump");
            lastHitTrap = collision.gameObject;
            runStatus = RUN_STATUS_BLOCK_JUMP;
        }
        else if (collision.gameObject.tag == "Down")
        {
            print("player hit Down");
            lastHitTrap = collision.gameObject;
            runStatus = RUN_STATUS_BLOCK_DOWN;
        }
    }

    public bool isRunning()
    {
        return speed >= MIN_SPEED;
    }

    public int getTotalStep()
    {
        return totalStep;
    }

    public IEnumerator ignorCollider(Collider collider)
    {
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), true);
        yield return new WaitForSeconds(2f);
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), false);
    }

    private void updateHeadStatus()
    {
        Vector3 headV = (head.transform.localPosition - lastHeadPosition) / Time.deltaTime;
        if (headV.y > HEAD_STEP_MIN_VELOCITY)
        {
            headStepDirection = STEP_DIRECTION_UP;
        }
        else if (headV.y < -HEAD_STEP_MIN_VELOCITY)
        {
            headStepDirection = STEP_DIRECTION_DOWN;
        }
        lastHeadPosition = head.transform.localPosition;
        if (headV.y > HEAD_JUMP_MIN_VELOCITY)
        {
            lastJumpTime = System.DateTime.Now;
            isHeadCrossJump = true;
            return;
        }
        else
        {
            System.TimeSpan duration = System.DateTime.Now - lastJumpTime;
            if (duration.TotalMilliseconds < CROSS_TRAP_DURATION_MILLI)
            {
                isHeadCrossJump = true;
                return;
            }
        }
        isHeadCrossJump = false;

        if (headV.y < HEAD_DOWN_MIN_VELOCITY || head.transform.localPosition.y < HEAD_DOWN_MIN_HEIGHT)
        {
            lastDownTime = System.DateTime.Now;
            isHeadCrossDown = true;
            return;
        }
        else
        {
            System.TimeSpan duration = System.DateTime.Now - lastDownTime;
            if (duration.TotalMilliseconds < CROSS_TRAP_DURATION_MILLI)
            {
                isHeadCrossDown = true;
                return;
            }
        }
        isHeadCrossDown = false;
    }

    private bool checkStep(System.TimeSpan duration)
    {
        bool haveStep = false;
        if (headStepDirection == STEP_DIRECTION_UP && lastStepDirection != STEP_DIRECTION_UP)
        {
            lastStepDirection = STEP_DIRECTION_UP;
            haveStep = updateStepCnt(duration);
        }
        else if (headStepDirection == STEP_DIRECTION_DOWN && lastStepDirection != STEP_DIRECTION_DOWN)
        {
            lastStepDirection = STEP_DIRECTION_DOWN;
            haveStep = updateStepCnt(duration);
        }
        return haveStep;
    }

    private bool updateStepCnt(System.TimeSpan duration)
    {
        bool haveStep = false;
        totalStep++;
        if (duration.TotalMilliseconds < STEP_MAX_DURATION_MILLI)
        {
            stepMoveCnt++;
            if (stepMoveCnt == START_MOVE_STEP)
            {
                runDirection = head.transform.forward;
                haveStep = true;
            }
            else if (stepMoveCnt > START_MOVE_STEP)
            {
                haveStep = true;
            }
        }
        else
        {
            stepMoveCnt = 1;
        }
        lastStepTime = System.DateTime.Now;
        return haveStep;
    }

}
