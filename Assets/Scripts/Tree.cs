using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isHit;
    private Vector3 spawnPos;

    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isHit = true;
            Destroy(gameObject, 5f);
        }
    }
    
    private void LateUpdate()
    {
        if (_isHit) return;
        Vector3 newRotation = _mainCamera.transform.eulerAngles;
        newRotation.x = 0;
        newRotation.z = 0;
        transform.eulerAngles = newRotation;
        transform.Rotate(Vector3.up, 90f);
    }
}
