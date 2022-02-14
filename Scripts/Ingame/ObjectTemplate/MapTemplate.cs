using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapTemplate : ScriptableObject
{
    public int mapCode;
    public int tilesetCode;
    public string[] mapGridLayout;
    public string[] spawnA;
    public string[] spawnB;
}
