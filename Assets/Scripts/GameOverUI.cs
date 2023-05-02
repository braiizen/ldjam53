using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text parcelsText;
    [SerializeField] private TMP_Text destroyedText;
    
    private void Update()
    {
        scoreText.text = $"Score: {GameManager.instance.score}";
        parcelsText.text = $"Parcels Delivered: {GameManager.instance.packagesDelivered}";
        destroyedText.text = $"Objects Destroyed: {GameManager.instance.objectsDestroyed}";
    }
}
