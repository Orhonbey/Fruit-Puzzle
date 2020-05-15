using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<StopPlatformController> stopPlatforms;

    public bool FailController()
    {
        foreach (var platform in stopPlatforms)
        {
            if (!platform.CounterController())
            {
                return false;
            }
        }
        return true;
    }
}