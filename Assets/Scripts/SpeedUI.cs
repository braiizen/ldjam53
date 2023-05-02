using System;
using TMPro;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text boostText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text deliveredText;
    
    void Update()
    {
        speedText.text = car.GetCurrentSpeed().ToString("0");
        boostText.text = GameManager.instance.fuel.ToString("0");
        timerText.text = String.Format("{0:0.00}", GameManager.instance.timer);
        scoreText.text = GameManager.instance.score.ToString();
        deliveredText.text = GameManager.instance.packagesDelivered.ToString();
    }
}
