using UnityEngine;
using TMPro;
using System.Collections;

public class PowerupMessage : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float blinkRate = 0.2f;
    [SerializeField] private Color upgradeColor = Color.yellow;
    [SerializeField] private Color unlockColor = Color.green;

    [Header("Message Formats")]
    [SerializeField] private string upgradeFormat = "{0} UPGRADED TO LEVEL {1}!";
    [SerializeField] private string unlockFormat = "{0} UNLOCKED!";

    private void Start()
    {
        if (messageText == null)
        {
            Debug.LogError("Message Text reference is missing in PowerupMessage!");
            return;
        }
        messageText.gameObject.SetActive(false);
    }

    public void ShowPowerupMessage(WeaponType weaponType, int newLevel, bool isUnlock)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(weaponType, newLevel, isUnlock));
    }

    private IEnumerator DisplayMessage(WeaponType weaponType, int newLevel, bool isUnlock)
    {
        messageText.gameObject.SetActive(true);
        messageText.color = isUnlock ? unlockColor : upgradeColor;
        
        string weaponName = GetWeaponDisplayName(weaponType);
        messageText.text = isUnlock 
            ? string.Format(unlockFormat, weaponName)
            : string.Format(upgradeFormat, weaponName, newLevel);

        float endTime = Time.time + displayDuration;
        bool isVisible = true;

        while (Time.time < endTime)
        {
            isVisible = !isVisible;
            messageText.enabled = isVisible;
            yield return new WaitForSeconds(blinkRate);
        }

        messageText.gameObject.SetActive(false);
    }

    private string GetWeaponDisplayName(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Missile => "MISSILE",
            WeaponType.Double => "DOUBLE SHOT",
            WeaponType.Twin => "TWIN SHOT",
            WeaponType.Option => "OPTION",
            WeaponType.Question => "SHIELD",
            WeaponType.Exclamation => "BOMB",
            _ => weaponType.ToString()
        };
    }
} 