using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    public static int count;
    public static Dictionary<Vector2, List<int>> gridList = new Dictionary<Vector2, List<int>>();
    private static List<GameObject> roomList = new List<GameObject>();
    public static List<GameObject> roomPool = new List<GameObject>();
    public static List<GameObject> roomListEnd = new List<GameObject>();
    public static Dictionary<int, int[]> compatibilityRef = new Dictionary<int, int[]>();
    //private static int roomCount;
    void Awake()
    {
        roomList.AddRange(Resources.LoadAll<GameObject>("Rooms"));
        for (int i = 0; i < roomList.Count; i++)
        {
            for (int x = 0; x < ((roomList[i].GetComponent<RoomInfo>().frequency* roomList[i].GetComponent<RoomInfo>().frequency)) + Random.Range(roomList[i].GetComponent<RoomInfo>().consistency * -2, roomList[i].GetComponent<RoomInfo>().consistency * 2); x++)
            {
                roomPool.Add(roomList[i]);
            }
        }
        compatibilityRef.Add(0, new int[] { 0, 2 });
        compatibilityRef.Add(1, new int[] { 1,});
        compatibilityRef.Add(2, new int[] { 2 });
        compatibilityRef.Add(9, new int[] { });
        Debug.Log(roomPool.Count);
        count = roomPool.Count;
        for (int i = 0; i < roomPool.Count; i++)
        {
            //Debug.Log(roomPool[i]);
        }
        roomListEnd.AddRange(Resources.LoadAll<GameObject>("End"));
        gridList.Add(new Vector2(0, 0), new List<int> { 0, 0, 0, 0 });
    }
}
