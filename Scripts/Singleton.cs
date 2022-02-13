using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance { 
        get {
            if (instance == null) {
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }


}
