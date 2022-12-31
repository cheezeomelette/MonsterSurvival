using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStat : Singleton<MyStat>
{
    [SerializeField] Image hpImage;
    [SerializeField] Text hpText;
    [SerializeField] Image steminaImage;
    [SerializeField] Text steminaText;

	// ü�� ������Ʈ
	public void UpdateHp(float currentHp, float maxHp)
	{
		hpImage.fillAmount = currentHp / maxHp;
		// �Ҽ��� �ø�
		hpText.text = Mathf.FloorToInt(currentHp).ToString();
	}

	public void UpdateStemina(float currentStemina, float maxStemina)
	{
		steminaImage.fillAmount = currentStemina / maxStemina;
		// �Ҽ��� �ø�
		steminaText.text = Mathf.FloorToInt(currentStemina).ToString();
	}
}
