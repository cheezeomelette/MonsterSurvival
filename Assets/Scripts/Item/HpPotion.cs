using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hp회복 포션
public class HpPotion : Item
{
	// 회복량
    [SerializeField] int healAmount;

	// 사용함수
	public override void UseItem(PlayerStatus status)
	{
        status.IncreaseHp(healAmount);
		// 아이템 갯수가 0개가 되면 오브젝트 파괴
		count -= 1;
		if (count <= 0)
			Destroy(gameObject);
	}
}
