using UnityEngine;
using System.Collections;

public class PathController : MonoBehaviour
{
    static public PathController Instance = null;
    PathController()
    {
        Instance = this;
    }

    public GameObject GetRandomPath()
    {
        PathData data = CsvParser.Instance.GetRandomPath();

        return Resources.Load(Config.PathPrefabFolder + data.Id) as GameObject;
    }
}
