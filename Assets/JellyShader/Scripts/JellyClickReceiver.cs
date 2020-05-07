using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyClickReceiver : MonoBehaviour
{
    RaycastHit hit;
    Ray clickRay;
    Renderer modelRenderer;
    public float timeFix;
    public float maxTimeCount;
    float currentTime;
    // Use this for initialization
    void Start()
    {
        modelRenderer = GetComponent<MeshRenderer>();
        currentTime = timeFix;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= maxTimeCount)
        {
            currentTime = timeFix;
        }
        currentTime += Time.deltaTime;
        modelRenderer.material.SetFloat("_ControlTime", currentTime);
    }
}
