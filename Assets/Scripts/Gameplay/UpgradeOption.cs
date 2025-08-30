[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public string upgradeDescription;
    public UpgradeType upgradeType;
    public float upgradeValue;

    public enum UpgradeType
    {
        MoveSpeed,
        FireSpeed,
        BulletSpeed,
        BulletDamage,
        Invulnerability
    }
    
    public void ApplyUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.MoveSpeed:
                Player.Instance.IncreaseMoveSpeed(upgradeValue);
                break;
            case UpgradeType.FireSpeed:
                Player.Instance.IncreaseFireSpeed(upgradeValue);
                break;
            case UpgradeType.BulletSpeed:
                Player.Instance.IncreaseBulletSpeed(upgradeValue);
                break;
            case UpgradeType.BulletDamage:
                Player.Instance.IncreaseBulletDamage((int)upgradeValue);
                break;
            case UpgradeType.Invulnerability:
                Player.Instance.IncreaseInvulnerability(upgradeValue);
                break;
        }
    }
}
