using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���׹̳� ����
public class SteminaPotion : Item
{
	[SerializeField] float healAmout;

	// ������ ���
	public override void UseItem(PlayerStatus status)
	{
		status.IncreaseStemina(healAmout);
		// ������ ������ 0���� �Ǹ� ������Ʈ �ı�
		count -= 1;
		if (count <= 0)
			Destroy(gameObject);
	}
}
