using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDescription : Singleton<WeaponDescription>
{
    [SerializeField] Image weaponSprite;
    [SerializeField] Text weaponName;
    [SerializeField] Text priceText;
    [SerializeField] Text powerText;
    [SerializeField] Text attackSpeedText;
    [SerializeField] Text reloadSpeedText;
    [SerializeField] Text bulletSpeedText;

	private void Start()
	{
        gameObject.SetActive(false);
	}
	public void SetItem(Gun gun)
    {
        weaponSprite.sprite = gun.weaponData.sprite;
        weaponName.text = gun.weaponData.weaponName;
        priceText.text = gun.weaponData.price.ToString();
        powerText.text = $"공격력 : {gun.weaponData.power.ToString()}";
        attackSpeedText.text = $"공격속도 : {gun.weaponData.fireRate.ToString()}";
        reloadSpeedText.text = $"재장전속도 : {gun.weaponData.reloadRate.ToString()}";
        bulletSpeedText.text = $"총알속도 : {gun.weaponData.bulletSpeed.ToString()}";
    }
}
