using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "LevelObstacleSO", menuName = "Scriptable Objects/LevelObstacleSO", order = 1)]
public class LevelObstacleSO : ScriptableObject
{
    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<Vector2> spawnPoints = new List<Vector2>();
    public List<Quaternion> spawnRotations = new List<Quaternion>();
}
