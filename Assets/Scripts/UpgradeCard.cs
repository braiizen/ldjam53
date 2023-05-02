using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering;

public class UpgradeCard
{
    public string UpgradeName;
    public float Multiplier;
    public string UpgradeType;
    public string Description => $@"Increases {UpgradeName} by {Multiplier}x";

    public UpgradeCard(string upgradeName, float multiplier, string upgradeType)
    {
        UpgradeName = upgradeName;
        Multiplier = multiplier;
        UpgradeType = upgradeType;
    }
}
