using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossGrenade : MonoBehaviour
{

    void Start()
    {
        GetComponent<UStickyBomb>().Release();
    }

    void Update()
    {
        
    }
}
