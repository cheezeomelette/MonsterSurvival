using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� �������� ����� ���� �߻�Ŭ����
public abstract class Item : MonoBehaviour
{
	// scriptableobject�� ������ ������ ������
	public ItemData itemData;
	// ������ ����
	public int count;

	// �����ۺ��� �ٸ� ȿ���� �������� �Լ�
	public abstract void UseItem(PlayerStatus status);
}
