using UnityEngine;
using System.Collections;

public class ReverseScript : MonoBehaviour {

    public GameObject enemy;
    public float reX;
    public float reZ;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemy.transform.position = new Vector3(reX,0,reZ);
            enemy.GetComponent<TitanFollowScript>().reverse = 1.5f;
            Destroy(this.gameObject);
        }
    }
}
