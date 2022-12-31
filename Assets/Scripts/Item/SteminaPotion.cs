using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테미너 포션
public class SteminaPotion : Item
{
	[SerializeField] float healAmout;

	// 아이템 사용
	public override void UseItem(PlayerStatus status)
	{
		status.IncreaseStemina(healAmout);
		// 아이템 갯수가 0개가 되면 오브젝트 파괴
		count -= 1;
		if (count <= 0)
			Destroy(gameObject);
	}
}
