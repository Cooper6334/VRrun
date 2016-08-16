using UnityEngine;
using System.Collections;

public class TitanFollowScript : MonoBehaviour {

    public Transform target;
    public Rigidbody myRidgetBody;
    public AudioSource runSound;
    public AudioSource attackSound;
    public AudioSource startSound;
    public Animation myAnimation;
    public float reverse = 1;

    Vector3 rotateOffset = Vector3.zero;
    bool isRunning = false;
    float speed;
    // Use this for initialization
    void Start () {
        myAnimation.Play("idle");

    }
	// Update is called once per frame
	void Update ()
    {
        if (!isRunning)
        {
            if (!myAnimation.IsPlaying("attack_2") && !myAnimation.IsPlaying("idle"))
            {
                myAnimation.Play("idle");
            }
            myRidgetBody.velocity = new Vector3(0, 0, 0);
            return;
        }
           
        Vector3 direction = target.position - transform.position;
        float speed = 5 * reverse;
        if (Vector3.Distance(target.position, transform.position) > 10)
        {
            speed = 15 * reverse;
        }
        else if (Vector3.Distance(target.position, transform.position) > 5)
        {
            speed = 7 * reverse;
        }
        myRidgetBody.velocity =( direction.normalized + rotateOffset) * speed;
        myRidgetBody.rotation = Quaternion.identity;
        Quaternion rockDirection = Quaternion.identity;
        float t = -Mathf.Atan2(direction.z,direction.x) * Mathf.Rad2Deg + 90;
        rockDirection.eulerAngles = new Vector3(0, t, 0);
        transform.rotation = rockDirection;
        transform.position = new Vector3( transform.position.x,0, transform.position.z);
    }
    public void startRun()
    {
        myAnimation.Play("run");
        startSound.Play();
        runSound.Play();
        //isRunning = true;
    }

    public void gameOver()
    {
        myAnimation.Play("attack_2");
        attackSound.PlayDelayed(0.3f);
        runSound.Stop();
        isRunning = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        print("collision "+ collision.gameObject.tag);
        if (collision.gameObject.tag == "Jump" || collision.gameObject.tag == "Down")
        {
            Destroy(collision.gameObject);
        }else if(collision.gameObject.tag == "Wall")
        {
            rotateOffset = collision.transform.forward;
                     
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rotateOffset = collision.transform.forward;
            print(rotateOffset);
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rotateOffset = Vector3.zero;
        }
    }
}
