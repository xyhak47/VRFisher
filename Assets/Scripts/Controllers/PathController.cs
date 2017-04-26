using UnityEngine;
using System.Collections;

public class PathController : MonoBehaviour
{
    static public PathController Instance = null;
    PathController()
    {
        Instance = this;
    }

    public GameObject GetRandomPath(FishData fishData)
    {
        PathData data = CsvParser.Instance.GetRandomPath(fishData);

        return Resources.Load(Config.PathPrefabFolder + data.Id) as GameObject;
    }
}
