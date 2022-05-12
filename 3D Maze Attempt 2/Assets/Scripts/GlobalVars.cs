using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    public static int count;
    public static Dictionary<Vector2, List<int>> gridList = new Dictionary<Vector2, List<int>>();
    public static List<GameObject> roomList20 = new List<GameObject>();
    public static List<GameObject> roomList50 = new List<GameObject>();
    public static List<GameObject> roomList30 = new List<GameObject>();
    public static List<GameObject> roomListEnd = new List<GameObject>();
    void Awake()
    {
        roomList20.AddRange(Resources.LoadAll<GameObject>("Rooms/20%"));
        roomList50.AddRange(Resources.LoadAll<GameObject>("Rooms/50%"));
        roomList30.AddRange(Resources.LoadAll<GameObject>("Rooms/30%"));
        roomListEnd.AddRange(Resources.LoadAll<GameObject>("Rooms/End"));
        gridList.Add(new Vector2(0, 0), new List<int> { 0, 0, 0, 0 });
    }
}
