using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �Ѿ��� �󸶳� ���Ҵ��� �˷��ִ� UI ������Ʈ Ǯ������ ����
public class BulletUI : MonoBehaviour
{
	// �Ѿ� ������ �� image�� RectTransform
	[SerializeField] RectTransform bullet;
	// ����� �Ѿ��� ���� ��Ȱ��ȭ Transform
	Transform disabledTransform;
	new Transform transform;

	// ���� �Ѿ˽���
	Stack<RectTransform> stack;
	// ����� �Ѿ� ����
	Stack<RectTransform> usedStack;
	RectTransform rect;
	int maxBullet;

	private void Awake()
	{
		// ĳ��
		transform = base.transform;
		rect = GetComponent<RectTransform>();

		// ���� ����
		stack = new Stack<RectTransform>();
		usedStack = new Stack<RectTransform>();

		// ����� �Ѿ��� ��Ƶ� BulletPool ����
		disabledTransform = new GameObject("BulletPool").transform;
		disabledTransform.SetParent(transform);
		disabledTransform.gameObject.SetActive(false);
	}

	// �ѱ�ü �� �����Ѿ� ������Ʈ
	public void ChangeWeapon(int current,int maxCount)	
	{
		maxBullet = maxCount;
		// �ִ��Ѿ˼��� �°� �Ѿ�UI ũ�� ����
		float size = (rect.sizeDelta.x / maxCount) - 3;
		bullet.sizeDelta = new Vector2(size, rect.sizeDelta.y);

		// ����� �Ѿ˿��� �Ѿ� ������
		while(usedStack.Count > 0)
		{
			GetBullet();
		}

		// ���ÿ� ����ִ� �Ѿ� ������ ����
		foreach (RectTransform rectTransform in stack)
		{
			rectTransform.sizeDelta = new Vector2(size, rect.sizeDelta.y);
		}

		// �Ѿ��� ���ڶ�� ���ο� �Ѿ� ����
		if (maxCount > stack.Count)
		{
			while (stack.Count < maxCount)
				AddBullet();
		}
		// �Ѿ��� ������ ����� �Ѿ˷� �ű�
		if(maxCount < stack.Count)
		{
			while(maxCount < stack.Count)
				ReturnBullet();
		}
		// �Ѿ��� ������ ����� �Ѿ˷� �ű�
		while (current < stack.Count)
			ReturnBullet();
	}

	// ���ο� �Ѿ� ����
	private void AddBullet()
	{
		stack.Push(Instantiate(bullet, transform));
	}
	// ����� �Ѿ˿��� ������
	private void GetBullet()
	{
		RectTransform bullet = usedStack.Pop();
		bullet.transform.SetParent(transform);
		stack.Push(bullet);
	}
	// ����� �Ѿ˷� �ݳ�
	private void ReturnBullet()
	{
		RectTransform bullet = stack.Pop();
		bullet.transform.SetParent(disabledTransform);
		usedStack.Push(bullet);
	}
	// �Ѿ� �߻� �� ui������Ʈ
	public void ShotBullet()
	{
		ReturnBullet();
	}
	// �������� �ִ밳����ŭ ���� ����
	public void Reload()
	{
		while (stack.Count < maxBullet)
			GetBullet();
	}
}
