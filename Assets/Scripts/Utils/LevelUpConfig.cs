using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpConfig", menuName = "Game/Level Up Config")]
public class LevelUpConfig : ScriptableObject
{
    public List<UpgradeOption> upgradeOptions = new List<UpgradeOption>
    {
        new UpgradeOption
        {
            upgradeName = "Move Speed +",
            upgradeDescription = "Increase movement speed by 1",
            upgradeType = UpgradeOption.UpgradeType.MoveSpeed,
            upgradeValue = 1f
        },
        new UpgradeOption
        {
            upgradeName = "Fire Rate +",
            upgradeDescription = "Decrease fire cooldown by 0.1s",
            upgradeType = UpgradeOption.UpgradeType.FireSpeed,
            upgradeValue = 0.1f
        },
        new UpgradeOption
        {
            upgradeName = "Move Speed ++",
            upgradeDescription = "Increase movement speed by 2",
            upgradeType = UpgradeOption.UpgradeType.MoveSpeed,
            upgradeValue = 2f
        },
        new UpgradeOption
        {
            upgradeName = "Bullet Speed +",
            upgradeDescription = "Increase bullet speed by 1",
            upgradeType = UpgradeOption.UpgradeType.BulletSpeed,
            upgradeValue = 1f
        },
        new UpgradeOption
        {
            upgradeName = "Bullet Speed ++",
            upgradeDescription = "Increase bullet speed by 2",
            upgradeType = UpgradeOption.UpgradeType.BulletSpeed,
            upgradeValue = 2f
        },
        new UpgradeOption
        {
            upgradeName = "Fire Rate ++",
            upgradeDescription = "Decrease fire cooldown by 0.2s",
            upgradeType = UpgradeOption.UpgradeType.FireSpeed,
            upgradeValue = 0.2f
        }
    };
}
