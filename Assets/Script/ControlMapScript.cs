using UnityEngine;
using System.Collections;

public class ControlMapScript : MonoBehaviour {

	enum Direction{left,up,right,down};

	private readonly int MAP_SIZE = 5;
	private readonly int MAP_CENTER = MAP_SIZE / 2 + 1;
    public GameObject fourWayPrefab;
	public GameObject threeWayPrefab;
	public GameObject twoWayJumpPrefab;
	public GameObject twoWayDownPrefab;
	public GameObject twoTurnWayPrefab;
	public GameObject rotatePrefab;
    public GameObject noRoadPrefab;

    Vector3 centerMapPosition;
    GameObject[] lastMap;
	MyMap[,] currentMap = new MyMap[MAP_SIZE,MAP_SIZE];

    // Use this for initialization
    void Start () {
        //initMap();
	}
    
    private void initMap()
    {
        // TODO add init center map
        createNewMap();
    }
    
    private void updateAllMap(int newCenterX,int newCenterZ)
    {
        clearOutMap(newCenterX, newCenterZ);
        createNewMap();
    }

    private void clearOutMap(int newCenterX, int newCenterZ)
    {
		int offsetX = newCenterX - MAP_CENTER;
		int offsetZ = newCenterZ - MAP_CENTER;
		MyMap[,] newMap = new MyMap[MAP_SIZE, MAP_SIZE];
		for(int i = 0; i < MAP_SIZE; i++)
        {
			for(int j = 0; j < MAP_SIZE; j++)
            {
				if (i + offsetX >= MAP_SIZE || i + offsetX < 0 || j + offsetZ >= MAP_SIZE || j + offsetZ < 0)
				{
					// old map out of bounds
                    Destroy(currentMap[i, j].mapObject);
                } else {
					// copt in bounds map to new map
                    newMap[i, j] = currentMap[i + offsetX, j + offsetZ];
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
            if (currentMap[x + 1, z] != null && currentMap[x + 1, z].canMoveLeft)
            {
				//road
				moveDirection[Direction.right] = 1;
            }
        }else
        {	//edge
			moveDirection[Direction.right] = 0;
        }
		if (x - 1 >= 0) {
			if (currentMap [x - 1, z] != null && currentMap [x - 1, z].canMoveRight) {
				moveDirection[Direction.left] = 1;				
			}

		} else {
			moveDirection[Direction.left] = 0;
		}
		if (z + 1 < 5) {
			if (currentMap [x, z + 1] != null && currentMap [x, z + 1].canMoveUp) {
				moveDirection[Direction.down] = 1;
			}
		} else {
			moveDirection[Direction.down] = 0;
		}
		if (z - 1 >= 0) {
			if (currentMap [x, z - 1] != null && currentMap [x, z - 1].canMoveDown) {
				moveDirection[Direction.up] = 1;
			}
		} else {
			moveDirection[Direction.up] = 0;
		}
		int haveMoveCount = 0;
		int canMoveCount = 0;
		for (int i = 0; i < 4; i++) {
			if (moveDirection [i] == 1) {
				haveMoveCount++;
				canMoveCount++;
			} else if (moveDirection == 0) {
				canMoveCount++;
			}
		}
		switch (haveMoveCount) {
		case 4:
		case 3:
			// create 4 way
			break;
		case 2:
			if (canMoveCount > haveMoveCount) {
				// create 3 way
			} else {
				// create 2 way
			}
			break;
		case 1:
			if (canMoveCount > haveMoveCount) {
				// create 2 way,3 way or rotate
			} else {
				// create rotate
			}
			break;
		case 0:
		default:
			// do nothing
			break;
		}
		return result;
    }

	private MyMap createNoRoadMap(int x,int z){
		MyMap result = null;
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
