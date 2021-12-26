using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int x { get; set; }
    public int y { get; set; }

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Point a, Point b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Point a, Point b) {
        return a.x != b.x || a.y != b.y;
    }
}
