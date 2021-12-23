using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableTiles : MonoBehaviour
{
    public bool isMouseOnCollider{ get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isMouseOnCollider = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter() {
        Debug.Log("enter");
    }

    private void OnMouseExit() {
        Debug.Log("exit");
    }
}
