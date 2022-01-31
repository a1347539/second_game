using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class ColorChanger : MonoBehaviour
{
    public void selectColor(int index) {

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        HelloWorldPlayer playerObject = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<HelloWorldPlayer>();

        playerObject.setColorServerRpc(this.gameObject.transform.GetChild(index).GetComponent<Image>().color);

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
