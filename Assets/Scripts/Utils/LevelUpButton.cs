using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public TMP_Text upgradeName;
    public TMP_Text upgradeDescription;
    public Image upgradeIcon;
    
    private UpgradeOption currentUpgrade;

    public void Setup(UpgradeOption upgrade)
    {
        currentUpgrade = upgrade;
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.upgradeDescription;
        if (upgrade.upgradeSprite != null) upgradeIcon.sprite = upgrade.upgradeSprite;
    }
    
    public void OnButtonClick()
    {
        if (currentUpgrade != null)
        {
            LevelUpSystem.Instance.ApplyUpgrade(currentUpgrade);
        }
    }
}
