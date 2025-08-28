using System.Collections.Generic;
using UnityEngine;

public class LevelUpSystem : MonoBehaviour
{
    public static LevelUpSystem Instance;
    
    [Header("UI References")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private LevelUpButton[] levelUpButtons;
    
    [Header("Level Up Configuration")]
    [SerializeField] private LevelUpConfig levelUpConfig;
    
    private List<UpgradeOption> currentOptions = new List<UpgradeOption>();
    
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    
    public void ShowLevelUpOptions()
    {
        currentOptions.Clear();
        
        currentOptions.AddRange(levelUpConfig.upgradeOptions);
        
        List<UpgradeOption> tempList = new List<UpgradeOption>(currentOptions);
        currentOptions.Clear();
        
        for (int i = 0; i < 3 && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            currentOptions.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        
        for (int i = 0; i < levelUpButtons.Length; i++)
        {
            if (i < currentOptions.Count)
            {
                levelUpButtons[i].gameObject.SetActive(true);
                levelUpButtons[i].Setup(currentOptions[i]);
            }
            else
            {
                levelUpButtons[i].gameObject.SetActive(false);
            }
        }
        
        PlayerUI.Instance.LevelUpPanelOpen();
    }
    
    public void HideLevelUpOptions()
    {
        PlayerUI.Instance.LevelUpPanelClose();
    }
    
    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        upgrade.ApplyUpgrade();
        HideLevelUpOptions();
        
        Debug.Log($"Applied upgrade: {upgrade.upgradeName}");
    }
}
