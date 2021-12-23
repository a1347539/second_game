using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Vector3 distanceToTravel;
    CharacterData characterData;

    // Start is called before the first frame update
    void Start()
    {
        characterData = GetComponent<CharacterData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move(Vector3 characterPosition, int vDistance, int hDistance) {
        if (Mathf.Abs(vDistance) > Mathf.Abs(hDistance)) { hDistance = 0; }
        else { vDistance = 0; }
        distanceToTravel = new Vector3(characterPosition.x + hDistance, 
            characterPosition.y + vDistance, 
            characterPosition.z);
        GetComponent<CharacterData>().deductEnergy(hDistance==0?Mathf.Abs(vDistance)*10:Mathf.Abs(hDistance)*10);

        transform.position = distanceToTravel;
    }
}
