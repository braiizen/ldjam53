using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    public UpgradeCard upgradeCard;

    public void SetUpgradeCard(UpgradeCard card)
    {
        upgradeCard = card;
        titleText.text = upgradeCard.UpgradeName;
        descriptionText.text = upgradeCard.Description;
    }
}
