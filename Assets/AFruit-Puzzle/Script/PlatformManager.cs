using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager ins;
    public List<Platform> platforms;
    [HideInInspector]
    public Platform active;
    private void Awake()
    {
        ins = this;
        active = platforms.Find(x => x.gameObject.activeSelf);
    }
}
