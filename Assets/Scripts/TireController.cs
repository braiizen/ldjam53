using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : MonoBehaviour
{
    [SerializeField] private Transform tireTransform;
    [SerializeField] private LayerMask groundLayer;
    public float raycastDistance = 0.5f;
    private TrailRenderer skidtrail;
    private AudioSource skidAudio;
    private bool skidFlag = false;


    private void Awake()
    {
        skidtrail = GetComponentInChildren<TrailRenderer>();
        skidAudio = GetComponent<AudioSource>();
    }


    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastDistance, groundLayer))
        {
            // If the ray hits something, set the tire transform's position to the hit point
            tireTransform.position = hit.point;
        }
    }

    public void StartEmitter()
    {
        if (skidFlag) return;
        skidtrail.emitting = true;
        skidAudio.Play();
        skidFlag = true;
    }

    public void StopEmitter()
    {
        if(!skidFlag) return;
        
        skidtrail.emitting = false;
        skidAudio.Stop();
        skidFlag = false;
    }
}


