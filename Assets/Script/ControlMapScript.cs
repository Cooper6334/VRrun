using UnityEngine;
using System.Collections;

public class ControlMapScript : MonoBehaviour {

	enum Direction{left,up,right,down};

	private static readonly int MAP_SIZE = 5;
	private static readonly int MAP_CENTER = MAP_SIZE / 2;
    private static readonly int MAP_WIDHT = 25;
    public GameObject startMap;
    public GameObject fourWayPrefab;
	public GameObject threeWayPrefab;
	public GameObject twoWayJumpPrefab;
	public GameObject twoWayDownPrefab;
	public GameObject twoWayTurnPrefab;
	public GameObject reversePrefab;
    public GameObject noRoadPrefab;

    Vector3 centerMapPosition;
    GameObject[] lastMap;
	MyMap[,] currentMap = new MyMap[MAP_SIZE,MAP_SIZE];
    int currentCenterX = 0;
    int currentCenterZ = 0;

    // Use this for initialization
    void Start () {
        initMap();
	}

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Map")
        {
            int newX = MAP_CENTER + ((int)(collider.transform.position.x - currentCenterX) / MAP_WIDHT);
            int newZ = MAP_CENTER + ((int)(collider.transform.position.z - currentCenterZ) / MAP_WIDHT);
            currentCenterX = (int)collider.transform.position.x;
            currentCenterZ = (int)collider.transform.position.z;
            updateAllMap(newX, newZ);
        }
    }
    
    private void initMap()
    {
        // TODO add init center map
        currentMap[MAP_CENTER, MAP_CENTER] = new MyMap(false,false,true,true);
        currentMap[MAP_CENTER, MAP_CENTER].mapObject = startMap;
        createNewMap();
    }
    
    private void updateAllMap(int newCenterX,int newCenterZ)
    {
        clearOutMap(newCenterX, newCenterZ);
        createNewMap();
    }

    private void clearOutMap(int newCenterX, int newCenterZ)
    {
		int offsetX = newCenterX - MAP_CENTER;//0
		int offsetZ = newCenterZ - MAP_CENTER;//1
		MyMap[,] newMap = new MyMap[MAP_SIZE, MAP_SIZE];
		for(int i = 0; i < MAP_SIZE; i++)
        {
			for(int j = 0; j < MAP_SIZE; j++)
            {
				if (i - offsetX >= MAP_SIZE || i - offsetX < 0 || j - offsetZ >= MAP_SIZE || j - offsetZ < 0)
				{
                    // old map out of bounds
                    Destroy(currentMap[i, j].mapObject);
                } else {
					// copt in bounds map to new map
                    newMap[i - offsetX, j - offsetZ] = currentMap[i,j];
                }
            }
        }
        currentMap = newMap;
    }

    private void createNewMap()
    {
		while (true) {
			bool updated = false;
			for (int i = 0; i < MAP_SIZE; i++) {
				for (int j = 0; j < MAP_SIZE; j++) {
					if (currentMap[i,j] == null && canReachMap (i, j)) {
						updated = true;
						currentMap [i, j] = createRoadMap (i, j);
					}
				}
			}
			if(!updated){
				break;
			}
		}
		for (int i = 0; i < MAP_SIZE; i++)
        {
			for (int j = 0; j < MAP_SIZE; j++)
            {
                if (currentMap[i,j] == null)
                {
                    currentMap[i, j] = createNoRoadMap(i, j);
                }
            }
        }
    }

	private MyMap createRoadMap(int x,int z)
    {
		MyMap result = null;
        // -1: can't go, 0:can go, 1: have go
		int[] moveDirection ={-1,-1,-1,-1};
        if (x + 1 < 5)
        {
            if (currentMap[x + 1, z] == null)
            {
                moveDirection[(int)Direction.right] = 0;
            }
            else if (currentMap[x + 1, z].canMoveLeft)
            {
                //road
                moveDirection[(int)Direction.right] = 1;
            }
        }else
        {	//edge
			moveDirection[(int)Direction.right] = 0;
        }
		if (x - 1 >= 0) {
            if (currentMap[x - 1, z] == null)
            {
                moveDirection[(int)Direction.left] = 0;
            }
             else   if (currentMap[x - 1, z].canMoveRight)
            { 
                //road
                moveDirection[(int)Direction.left] = 1;               
			}
		} else {
			moveDirection[(int)Direction.left] = 0;
		}
		if (z + 1 < 5) {
            if (currentMap[x, z + 1] == null)
            {
                moveDirection[(int)Direction.down] = 0;
            }
            else if (currentMap[x, z + 1].canMoveUp)
            {
                //road
                moveDirection[(int)Direction.down] = 1;
            }
		} else {
			moveDirection[(int)Direction.down] = 0;
		}
		if (z - 1 >= 0) {
            if (currentMap[x, z - 1] == null)
            {
                moveDirection[(int)Direction.up] = 0;
            }
            else if (currentMap[x, z - 1].canMoveDown)
            {
                //road
                moveDirection[(int)Direction.up] = 1;
            }
		} else {
			moveDirection[(int)Direction.up] = 0;
		}
		int haveMoveCount = 0;
		int canMoveCount = 0;
		for (int i = 0; i < 4; i++) {
			if (moveDirection [i] == 1) {
				haveMoveCount++;
				canMoveCount++;
			} else if (moveDirection[i] == 0) {
				canMoveCount++;
			}
		}
        // TODO: handle rotate map
		switch (haveMoveCount) {
		case 4:
		case 3:
                // create 4 way
                result = new MyMap(true, true, true, true);
                result.mapObject = (GameObject)Instantiate(fourWayPrefab);
                result.mapObject.transform.position = new Vector3(currentCenterX+ MAP_WIDHT * (x-MAP_CENTER),0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
            break;
		case 2:
			if (canMoveCount > haveMoveCount) {
                    // create 3 way
                    result = new MyMap(false, true, true, true);
                    result.mapObject = (GameObject)Instantiate(threeWayPrefab);
                    result.mapObject.transform.position = new Vector3(currentCenterX + MAP_WIDHT * (x - MAP_CENTER), 0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
                } else {
                    // create 2 way
                    result = new MyMap(false, false, true, true);
                    result.mapObject = (GameObject)Instantiate(twoWayJumpPrefab);
                    result.mapObject.transform.position = new Vector3(currentCenterX + MAP_WIDHT * (x - MAP_CENTER), 0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
                }
			break;
		case 1:
			if (canMoveCount > haveMoveCount) {
                    // create 2 way,3 way or rotate
                    result = new MyMap(false, false, true, true);
                    result.mapObject = (GameObject)Instantiate(twoWayJumpPrefab);
                    result.mapObject.transform.position = new Vector3(currentCenterX + MAP_WIDHT * (x - MAP_CENTER), 0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
                } else {
                    // create rotate
                   result = new MyMap(false, false, false, true);
                    result.mapObject = (GameObject)Instantiate(reversePrefab);
                    result.mapObject.transform.position = new Vector3(currentCenterX + MAP_WIDHT * (x - MAP_CENTER), 0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
                }
			break;
		case 0:
		default:
			// do nothing
			break;
		}
        if (result != null)
        {
            Transform child = result.mapObject.transform.FindChild("Tree");
            child.gameObject.SetActive(false);
        }
		return result;
    }

	private MyMap createNoRoadMap(int x,int z)
    {        
        MyMap result = new MyMap(false, false, false, false);
        result.mapObject = (GameObject)Instantiate(noRoadPrefab);
        result.mapObject.transform.position = new Vector3(currentCenterX + MAP_WIDHT * (x - MAP_CENTER), 0, currentCenterZ + MAP_WIDHT * (z - MAP_CENTER));
        if (result != null)
        {
            Transform child = result.mapObject.transform.FindChild("Tree");
            child.gameObject.SetActive(false);
        }
        return result;
	}
    
    private bool canReachMap(int x,int z)
    {
		if (x + 1 < MAP_SIZE)
        {
            if (currentMap[x + 1, z] != null && currentMap[x + 1, z].canMoveLeft)
            {
                return true;
            }
        }
        if (x - 1 >= 0)
        {
            if (currentMap[x - 1, z] != null && currentMap[x - 1, z].canMoveRight)
            {
                return true;
            }
        }
		if (z + 1 < MAP_SIZE)
        {
            if (currentMap[x , z + 1] != null && currentMap[x, z + 1].canMoveUp)
            {
                return true;
            }
        }
        if (z - 1 >= 0)
        {
            if (currentMap[x, z - 1] != null && currentMap[x, z - 1].canMoveDown)
            {
                return true;
            }
        }
        return false;
    }

    class MyMap
    {
        public GameObject mapObject;
        public bool canMoveLeft;
        public bool canMoveRight;
        public bool canMoveUp;
        public bool canMoveDown;

        public MyMap(bool left, bool right, bool up, bool down)
        {
            canMoveLeft = left;
            canMoveRight = right;
            canMoveUp = up;
            canMoveDown = down;           
        }
    }
}
