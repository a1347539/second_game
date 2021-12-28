using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerContainer;
    [SerializeField] Character player_1;
    public List<Character> character;
    private int[][] spawnA;
    
    private void Awake() {
        character = new List<Character>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnA = MapBuilder.Instance.mapData.spawnA;
        spawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnPlayers() {
        Character newCharacter = Instantiate(player_1).GetComponent<Character>();
        newCharacter.setup(spawnA[0], playerContainer);
        character.Add(newCharacter);
    }
}
