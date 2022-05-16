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
    private int randomNum;

    void Start()
    {
        if (GlobalVars.count > 80) { end = true; }
        thisRoomInfo = gameObject.GetComponent<RoomInfo>();
        for (int i = 0; i < 4; i++)
        {
            if (spawnedFrom == i)
            {
                continue;
            }
            //Stops room from performing any checks on the room it was spawned from.

            if (i == 0) { zChange = 1; xChange = 0; }
            else if (i == 1) { xChange = 1; zChange = 0; }
            else if (i == 2) { zChange = -1; xChange = 0; }
            else { xChange = -1; zChange = 0; }
            //A set of variables sets that dictate which side of the room is being checked.

            try
            {
                List<int> temp = GlobalVars.gridList[new Vector2(thisRoomInfo.gridNumX + xChange, thisRoomInfo.gridNumZ + zChange)];
                continue;
            }
            catch { }
            //A try and catch to check if the current side of the room we're looking at already has a room placed there, if it does skip this side.
            if (thisRoomInfo.outerWalls[i] == 1)
            {
                continue;
            }
            //If the current room has a wall of 1 (outer wall) skip it.
            complementWall = i + 2;
            if (complementWall > 3)
            {
                complementWall -= 4;
            }
            //Calculates what the complement wall number is essentially it finds the opposite of the current direction e.g. north == 0, so compliemnt wall would find the number that represents south, which is 3.
            validTiles.Clear();
            if(end == true)
            {
                thisRoomList = GlobalVars.roomListEnd;
            }
            else
            {
                thisRoomList = GlobalVars.roomPool;
            }

            for (int x = 0; x < thisRoomList.Count; x++)
            {
                validTiles.Add(thisRoomList[x]);
            }
            randomNum = Random.Range(0, thisRoomList.Count);
            if(end == false)
            {
                GlobalVars.roomPool.RemoveAt(randomNum);
            }
            tempPrefab = Instantiate(validTiles[randomNum], new Vector3(gameObject.transform.position.x + (xChange * gameObject.transform.localScale.x), gameObject.transform.position.y, gameObject.transform.position.z + (zChange * gameObject.transform.localScale.z)), Quaternion.identity);
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
    public bool checkCompatibility(List<int> walls, int wallType)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            if(walls[i] == wallType)
            {
                return true;
            }
        }
        return false;
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
