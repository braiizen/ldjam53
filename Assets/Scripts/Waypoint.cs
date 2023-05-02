using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Screen;

public class Waypoint : MonoBehaviour
{
    public Image img;
    public Transform target;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private void Awake()
    {
        minX = img.GetPixelAdjustedRect().width / 2;
        maxX = width - minX;

        minY = img.GetPixelAdjustedRect().height / 2;
        maxY = height - minY;
    }

    private void Update()
    {
        
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }
        
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
    }
}
