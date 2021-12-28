using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Point point { get; set; }

    public void setup(Vector3 worldPoint, int[] point, Transform container) {
        this.point = new Point(point[0], point[1]);
        transform.position = worldPoint;
        transform.parent = container;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
