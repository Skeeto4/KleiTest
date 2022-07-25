using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTurnerTest : MonoBehaviour
{
    private string READ_PATH => Application.streamingAssetsPath + "/test.txt";
    private string WRITE_PATH => Application.streamingAssetsPath + "/result.txt";

    void Start()
    {
        WeaponTurner.TuneWeapons(READ_PATH, WRITE_PATH);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
