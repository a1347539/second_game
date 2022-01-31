using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SpawnableCircle : NetworkBehaviour
{
    private NetworkVariable<Color> ballColor = new NetworkVariable<Color>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        ballColor.Value = UnityEngine.Random.ColorHSV();
    }

    private void OnEnable()
    {
        ballColor.OnValueChanged += onBallValueChanged;
    }

    private void OnDisable()
    {
        ballColor.OnValueChanged -= onBallValueChanged;
    }

    private void onBallValueChanged(Color previousValue, Color newValue)
    {
        if (!IsClient) { return; }
        this.GetComponent<SpriteRenderer>().color = newValue;
    }
}
