using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineGenerator : MonoBehaviour
{

    public static CoroutineGenerator instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void _StartCoroutine(IEnumerator iEnumerator)
    {
        StartCoroutine(iEnumerator);
    }
}