using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{
    public int spawnedFrom;
    private GameObject tempPrefab;
    private int xChange;
    //private int yChange;
    private int zChange;
    private int numRotated;
    private int dodged;
    private RoomInfo thisRoomInfo;
    private RoomInfo tempRoomInfo;
    private WallPositions wallPos;
    private int complementWall;
    private List<GameObject> thisRoomList = new List<GameObject>();
    private List<GameObject> validTiles = new List<GameObject>();
    private List<GameObject> wallList = new List<GameObject>();
    private int rotation;
    private int pushDirection;
    private bool noDodging;
    private bool end;
    private int randomPercent;

    void Start()
    {
        if (GlobalVars.count > 100) { end = true; }
        thisRoomInfo = gameObject.GetComponent<RoomInfo>();
        for (int i = 0; i < 4; i++)
        {
            if (spawnedFrom == i)
            {
                continue;
            }

            if (i == 0) { zChange = 1; xChange = 0; }
            else if (i == 1) { xChange = 1; zChange = 0; }
            else if (i == 2) { zChange = -1; xChange = 0; }
            else { xChange = -1; zChange = 0; }

            try
            {
                List<int> temp = GlobalVars.gridList[new Vector2(thisRoomInfo.gridNumX + xChange, thisRoomInfo.gridNumZ + zChange)];
                continue;
            }
            catch { }
            if (thisRoomInfo.outerWalls[i] == 1)
            {
                continue;
            }
            complementWall = i + 2;
            if (complementWall > 3)
            {
                complementWall -= 4;
            }
            validTiles.Clear();
            randomPercent = Random.Range(0, 101);
            if(end == true)
            {
                thisRoomList = GlobalVars.roomListEnd;
            }
            else
            {
                if (randomPercent <= 20)
                {
                    thisRoomList = GlobalVars.roomList20;
                }
                else if (randomPercent > 20 && randomPercent <= 50)
                {
                    thisRoomList = GlobalVars.roomList30;
                }
                else
                {
                    thisRoomList = GlobalVars.roomList50;
                }
            }

            for (int x = 0; x < thisRoomList.Count; x++)
            {
                validTiles.Add(thisRoomList[x]);
            }
            tempPrefab = Instantiate(validTiles[Random.Range(0, validTiles.Count)], new Vector3(gameObject.transform.position.x + (xChange * gameObject.transform.localScale.x), gameObject.transform.position.y, gameObject.transform.position.z + (zChange * gameObject.transform.localScale.z)), Quaternion.identity);
            tempPrefab.GetComponent<RoomSpawn>().spawnedFrom = complementWall;

            tempRoomInfo = tempPrefab.GetComponent<RoomInfo>();
            numRotated = 0;
            pushDirection = Random.Range(0, 2);
            dodged = 0;
            noDodging = false;
            while (true)
            {
                if(tempRoomInfo.outerWalls[complementWall] != 1)
                {
                    if(noDodging == true || dodged >= 2)
                    {
                        break;
                    }
                }
                if(tempRoomInfo.outerWalls[complementWall] != 1 && Random.Range(0,3) == 1 && dodged < 2)
                {
                    dodged += 1;
                    tempRoomInfo.outerWalls = pushList(tempRoomInfo.outerWalls, pushDirection);
                    numRotated += 1;
                }
                if (tempRoomInfo.outerWalls[complementWall] == 1)
                {
                    noDodging = true;
                    tempRoomInfo.outerWalls = pushList(tempRoomInfo.outerWalls, pushDirection);
                    numRotated += 1;
                }
            }
            tempPrefab.transform.rotation = Quaternion.Euler(0, rotation * numRotated, 0);
            tempRoomInfo.gridNumX = thisRoomInfo.gridNumX + xChange;
            tempRoomInfo.gridNumZ = thisRoomInfo.gridNumZ + zChange;
            GlobalVars.gridList.Add(new Vector2(tempRoomInfo.gridNumX, tempRoomInfo.gridNumZ), tempRoomInfo.outerWalls);
            GlobalVars.count += 1;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i == 0) { zChange = 1; xChange = 0; }
            else if (i == 1) { xChange = 1; zChange = 0; }
            else if (i == 2) { zChange = -1; xChange = 0; }
            else { xChange = -1; zChange = 0; }
            complementWall = i + 2;
            if (complementWall > 3)
            {
                complementWall -= 4;
            }
            try
            {
                if (GlobalVars.gridList[new Vector2(thisRoomInfo.gridNumX + xChange, thisRoomInfo.gridNumZ + zChange)][complementWall] != 0)
                {
                    spawnWalls(i, GlobalVars.gridList[new Vector2(thisRoomInfo.gridNumX + xChange, thisRoomInfo.gridNumZ + zChange)][complementWall]);
                    continue;
                }
            }
            catch { }
        }
    }
    public bool checkName(string name)
    {
        if(name[0].ToString() == "E" && name[1].ToString() == "N" && name[2].ToString() == "D")
        {
            return true;
        }
        return false;
    }
    public List<int> pushList(List<int> wallList, int direction)
    {
        int tempInt = 0;
        if (direction == 0)
        {
            rotation = -90;
            for (int i = 0; i < 3; i++)
            {
                tempInt = wallList[i];
                wallList[i] = wallList[i + 1];
                wallList[i + 1] = tempInt;
            }
        }
        else if (direction == 1)
        {
            rotation = 90;
            for (int i = 3; i > 0; i--)
            {
                tempInt = wallList[i];
                wallList[i] = wallList[i - 1];
                wallList[i - 1] = tempInt;
            }
        }
        return (wallList);
    }
    public void spawnWalls(int i, int wallType)
    {
        wallList.Clear();
        if(wallType == 1)
        {
            wallList.AddRange(Resources.LoadAll<GameObject>("OuterWalls"));
        }
        else if(wallType == 2)
        {
            wallList.AddRange(Resources.LoadAll<GameObject>("CenterWalls"));
        }
        if (i == 0)
        {
            wallPos = wallList[1].GetComponent<WallPositions>();
            Instantiate(wallList[1], new Vector3(gameObject.transform.position.x + wallPos.xPosition, wallPos.yPosition, gameObject.transform.position.z + wallPos.zPosition), Quaternion.Euler(0, 90, 0));
        }
        else if (i == 1)
        {
            wallPos = wallList[0].GetComponent<WallPositions>();
            Instantiate(wallList[0], new Vector3(gameObject.transform.position.x + wallPos.xPosition, wallPos.yPosition, gameObject.transform.position.z + wallPos.zPosition), Quaternion.Euler(0, 90, 0));
        }
        else if (i == 2)
        {
            wallPos = wallList[1].GetComponent<WallPositions>();
            Instantiate(wallList[1], new Vector3(gameObject.transform.position.x + wallPos.xPosition, wallPos.yPosition, gameObject.transform.position.z - wallPos.zPosition), Quaternion.Euler(0, 90, 0));
        }
        else if (i == 3)
        {
            wallPos = wallList[0].GetComponent<WallPositions>();
            Instantiate(wallList[0], new Vector3(gameObject.transform.position.x - wallPos.xPosition, wallPos.yPosition, gameObject.transform.position.z + wallPos.zPosition), Quaternion.Euler(0, 90, 0));
        }
    }

}
