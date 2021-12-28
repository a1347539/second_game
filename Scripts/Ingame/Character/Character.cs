using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Point point { get; private set; }

    public void setup(int[] spawnPoint, Transform container) {
        this.point = new Point(spawnPoint[0], spawnPoint[1]);
        transform.position = new Vector3(
            MapBuilder.Instance.origin.x + MapBuilder.Instance.tileWidth * ( spawnPoint[0] - 0.5f),
            MapBuilder.Instance.origin.y - MapBuilder.Instance.tileHeight * ( spawnPoint[1] - 0.5f),
            0
        );
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
