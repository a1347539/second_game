using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int x { get; set; }
    public int y { get; set; }

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Vector3 toWorldPosition() {
        return new Vector3(
           MapBuilder.Instance.origin.x + MapBuilder.Instance.tileWidth * (x + 0.5f),
           MapBuilder.Instance.origin.y - MapBuilder.Instance.tileHeight * (y + 0.5f),
           0
       );
    }

    public string print() {
        return (x.ToString() + " " + y.ToString());
    }

    public static bool operator == (Point a, Point b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator != (Point a, Point b) {
        return a.x != b.x || a.y != b.y;
    }
}
