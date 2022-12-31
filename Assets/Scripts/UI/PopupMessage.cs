using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : Singleton<PopupMessage>
{
	[SerializeField] CanvasGroup group;
    [SerializeField] Text commentText;
	[SerializeField] float disappearTime;

	private void Start()
	{
		group.alpha = 0f;
	}

	// �˾� �����ֱ� �Լ�
	public void Show(string comment, float showTime)
	{
		commentText.text = comment;
		StartCoroutine(ShowPopup(showTime));
	}

	// showTime���� �޽����� �����ش�
	IEnumerator ShowPopup(float showTime)
	{
		group.alpha = 1f;
		yield return new WaitForSeconds(showTime);
		while(group.alpha > 0)
		{
			group.alpha -= Time.deltaTime / disappearTime;
			yield return null;
		}
		group.alpha = 0f;
	}
}
