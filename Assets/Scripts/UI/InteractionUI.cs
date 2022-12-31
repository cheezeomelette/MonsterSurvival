using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ȣ�ۿ�UI ����
public class InteractionUI : Singleton<InteractionUI>
{
	[SerializeField] GameObject panel;
	Camera cam;

	private void Start()
	{
		cam = Camera.main;
		panel.SetActive(false);
	}

	// ��ȣ�ۿ�UI ��ġ�� ������Ʈ
	public void UpdateUI(IInteraction target)
	{
		panel.SetActive(true);
		Vector3 screenPoint = cam.WorldToScreenPoint(target.Position);

		// �ڸ� ������ �� ���̴� UI�� ȭ������� ���� �Ⱥ��̰� ����
		if(screenPoint.z < 0f)
		{
			screenPoint = new Vector3(-100, -100, 0);
		}
		transform.position = screenPoint;
	}

	// UI����
	public void OffUI()
	{
		panel.SetActive(false);
	}
}
