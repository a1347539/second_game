using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableTiles : Tiles
{

    public override void setup(Point point, Vector3 position, Transform container) {
        this.point = point;
        transform.position = position;
        transform.parent = container;
        base.isWalkable = false;
        this.name = point.print();
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
