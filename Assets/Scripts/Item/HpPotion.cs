using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hpȸ�� ����
public class HpPotion : Item
{
	// ȸ����
    [SerializeField] int healAmount;

	// ����Լ�
	public override void UseItem(PlayerStatus status)
	{
        status.IncreaseHp(healAmount);
		// ������ ������ 0���� �Ǹ� ������Ʈ �ı�
		count -= 1;
		if (count <= 0)
			Destroy(gameObject);
	}
}
