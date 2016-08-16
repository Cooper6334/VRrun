using UnityEngine;
using System.Collections;

public class RunnerScript : MonoBehaviour {

    public Transform nextRunPoint;
    public Rigidbody myRigidBody;
    public Transform lastRunPoint;
    public Animator myAnimator;
    public Transform playerTransform;
    public ChaseGameScript gameScript;
    public AudioSource jumpAudio;
    public AudioSource downAudio;
    public AudioSource hitAudio;

    private static readonly float MIN_SPEED = 1f;
    private static readonly float MAX_SPEED_FAR = 2f;
    private static readonly float MAX_SPEED_NORMAL = 3f;
    private static readonly float MAX_SPEED_NEAR = 5f;
    private static readonly float ADD_SPEED = 0.1f;
    private static readonly float MINUS_SPEED = 0.05f;
    private static readonly float jumpSpeed = 2f;
    private static readonly float downSpeed = 2f;

    Vector3 targetDirection;
    bool isCrossing = false;
    bool isRunning = false;
    public float speed = 0;

    // Use this for initialization
    void Start() {
        lastRunPoint = transform;
    }

    // Update is called once per frame
    void Update() {
        updateTargetRunPoint();
    }

    public void startRun()
    {
        isRunning = true;
        myAnimator.SetBool("run", true);
    }

    public void updateTargetRunPoint()
    {
        if (!isRunning)
        {
            return;
        }
        if (!isCrossing)
        {
            updateRunSpeed();
        }
        targetDirection = (nextRunPoint.position - transform.position).normalized;
        myRigidBody.velocity = targetDirection * speed;
        Quaternion direction = Quaternion.identity;
        float t = -Mathf.Atan2(targetDirection.z, targetDirection.x) * Mathf.Rad2Deg + 90;
        direction.eulerAngles = new Vector3(0, t, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.deltaTime * 5);
    }
    public void updateTargetRunPoint(Transform newRunPoint)
    {
        lastRunPoint = nextRunPoint;
        nextRunPoint = newRunPoint;
        updateTargetRunPoint();
        gameScript.checkGameOver(lastRunPoint);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isCrossing && collision.gameObject.tag == "Jump")
        {
            print("npc hit Jump "+collision.gameObject.name);
            StartCoroutine(ignoreJumpCollider(collision.gameObject.GetComponent<Collider>()));
        }else if(!isCrossing && collision.gameObject.tag == "Down")
        {
            print("npc hit Down " + collision.gameObject.name);
            StartCoroutine(ignoreDownCollider(collision.gameObject.GetComponent<Collider>()));            
        }else if(collision.gameObject.tag == "Player")        
        {
            hitAudio.Play();
        }
    }

    public IEnumerator ignoreJumpCollider(Collider collider)
    {
        isCrossing = true;
        myAnimator.SetBool("jump", true);
        speed = jumpSpeed;
        yield return new WaitForSeconds(0.2f);
        jumpAudio.Play();
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), true);
        yield return new WaitForSeconds(0.5f);
        myAnimator.SetBool("jump", false);
        isCrossing = false;
        yield return new WaitForSeconds(2f);
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), false);
    }

    public IEnumerator ignoreDownCollider(Collider collider)
    {
        isCrossing = true;
        myAnimator.SetBool("down", true);
        speed = downSpeed;
        yield return new WaitForSeconds(0.2f);
        downAudio.Play();
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), true);
        yield return new WaitForSeconds(0.2f);
        myAnimator.SetBool("down", false);
        isCrossing = false;
        yield return new WaitForSeconds(0.8f);
        Physics.IgnoreCollision(collider, GetComponent<Collider>(), false);
    }
    private void updateRunSpeed()
    {
        float targetSpeed;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance < 5)
        {
            targetSpeed = MAX_SPEED_NEAR;
        } else if (distance < 10)
        {
            targetSpeed = MAX_SPEED_NORMAL;
        } else
        {
            targetSpeed = MAX_SPEED_FAR;
        }

        if(speed < targetSpeed)
        {
            speed += ADD_SPEED;
        }else if(speed > targetSpeed)
        {
            speed -= MINUS_SPEED;
        }
        
    }
}
