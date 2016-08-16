using UnityEngine;
using System.Collections;

public class ControlMapScript : MonoBehaviour {
    
    public GameObject[] mapPrefabs;
    public GameObject noRoadPrefab;

    Vector3 centerMapPosition;
    GameObject[] lastMap;
    MyMap[,] currentMap = new MyMap[5,5];

    // Use this for initialization
    void Start () {
        //initMap();
	}


    /*
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
        int offsetX = newCenterX - 3;
        int offsetZ = newCenterZ = 3;
        MyMap[,] newMap = new MyMap[5, 5];
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (i + offsetX >= 5 || i + offsetX < 0 || j + offsetZ >= 5 || j + offsetZ < 0)
                {
                    Destroy(currentMap[i, j].mapObject);
                } else {
                    newMap[i, j] = currentMap[i + offsetX, j + offsetZ];
                }
            }
        }
        currentMap = newMap;
    }
    private void createNewMap()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if (canReachMap(i, j))
                {
                    currentMap[i, j] = createRandomMap(i, j);
                }
            }
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (currentMap[i,j] == null)
                {
                    currentMap[i, j] = createNoRoadMap(i, j);
                }
            }
        }

    }
    private MyMap createRandomMap(int x,int z)
    {
        // -1: can't go, 0:can go, 1: have go
        int moveLeft = -1;
        int moveRight = -1;
        int moveUp = -1;
        int moveDown = -1;
        if (x + 1 < 5)
        {
            if (currentMap[x + 1, z] != null && currentMap[x + 1, z].canMoveLeft)
            {
                moveRight = 1;
            }
        }else
        {
            moveRight = 0;
        }
        if (x - 1 >= 0)
        {
            if (currentMap[x - 1, z] != null && currentMap[x - 1, z].canMoveRight)
            {
                return true;
            }

        }
        if (z + 1 < 5)
        {
            if (currentMap[x, z + 1] != null && currentMap[x, z + 1].canMoveDown)
            {
                return true;
            }
        }
        if (z - 1 >= 0)
        {
            if (currentMap[x, z - 1] != null && currentMap[x, z - 1].canMoveUp)
            {
                return true;
            }
        }
    }
    */
    private bool canReachMap(int x,int z)
    {
        if (x + 1 < 5)
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
        if (z + 1 < 5)
        {
            if (currentMap[x , z + 1] != null && currentMap[x, z + 1].canMoveDown)
            {
                return true;
            }
        }
        if ( z - 1 >= 0)
        {
            if (currentMap[x, z - 1] != null && currentMap[x, z - 1].canMoveUp)
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
