using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���������� ��� ���������̺�
[CreateAssetMenu(fileName = "ItemData", menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
	public Sprite sprite;
	public string weaponName;
	public string description;
	public int maxBulletCount;
	public int price;
	public float reloadRate;
	public float fireRate;
	public float bulletSpeed;
	public float power;
}
