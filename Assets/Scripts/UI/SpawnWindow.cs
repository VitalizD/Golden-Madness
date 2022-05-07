using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWindow : MonoBehaviour
{
    public void SpawnNeedWindow(GameObject objPrefab) 
    {
        Instantiate(objPrefab);
    }
    
}
