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
        powerText.text = $"���ݷ� : {gun.weaponData.power.ToString()}";
        attackSpeedText.text = $"���ݼӵ� : {gun.weaponData.fireRate.ToString()}";
        reloadSpeedText.text = $"�������ӵ� : {gun.weaponData.reloadRate.ToString()}";
        bulletSpeedText.text = $"�Ѿ˼ӵ� : {gun.weaponData.bulletSpeed.ToString()}";
    }
}
