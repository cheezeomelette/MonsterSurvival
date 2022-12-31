using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : Singleton<Score>
{
    [SerializeField] Text attackText;
    [SerializeField] Text damagedText;
    [SerializeField] Text killText;
    [SerializeField] Text deathText;
    [SerializeField] public GameObject Panel;

    float attack = 0;
    int kill = 0;
    float damaged = 0;
    int death = 0;

	private void Start()
	{
        Panel.SetActive(false);
	}
	public void AddDamage(float amount)
	{
        attack += amount;
    }

    public void AddKill()
	{
        kill += 1;
	}

    public void GetDamaged(float amount)
	{
        damaged += amount;
	}

    public void Die()
	{
        death += 1;
	}

    public void ShowResult()
	{
        Panel.SetActive(true);
        attackText.text = attack.ToString();
        killText.text = kill.ToString();
        damagedText.text = damaged.ToString();
        deathText.text = death.ToString();
    }
}
