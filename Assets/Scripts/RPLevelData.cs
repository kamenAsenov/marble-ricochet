using System.Collections.Generic;
using UnityEngine;

public enum RPObstacleType
{
    NormalWall,
    MetalBumper,
    BreakableGlass
}

[System.Serializable]
public class RPLevelData
{
    public int levelNumber;
    public string title;
    public Vector3 ballStart;
    public int parShots = 1;
    public List<Vector3> targets = new List<Vector3>();
    public List<RPWallData> walls = new List<RPWallData>();
}

[System.Serializable]
public class RPWallData
{
    public Vector3 position;
    public Vector3 scale;
    public RPObstacleType type;

    public RPWallData(Vector3 position, Vector3 scale, RPObstacleType type = RPObstacleType.NormalWall)
    {
        this.position = position;
        this.scale = scale;
        this.type = type;
    }
}