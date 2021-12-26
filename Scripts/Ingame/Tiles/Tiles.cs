using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Point point { get; protected set; }

    public virtual void setup(Point point, Vector3 position, Transform parent) {
        this.point = point;
        transform.position = position;
        transform.parent = parent;
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
