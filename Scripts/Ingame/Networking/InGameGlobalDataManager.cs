using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InGameGlobalDataManager : MonoBehaviour
{
    public static InGameGlobalDataManager Instance => instance;
    private static InGameGlobalDataManager instance;
    public NetworkList<InGamePlayerState> inGamePlayers = new NetworkList<InGamePlayerState>();
    public Dictionary<ulong, NetworkObject> playerObjectDict = new Dictionary<ulong, NetworkObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
