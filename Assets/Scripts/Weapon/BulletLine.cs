using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLine : MonoBehaviour
{
	float deadLine = 2f;

	public void Shoot(Vector3 dir)
	{
		StartCoroutine(shoot(dir));
	}
	IEnumerator shoot(Vector3 dir)
	{
		float currentTime = 0f;
		while(deadLine > currentTime)
		{
			transform.position += dir.normalized * 1000 * Time.deltaTime;
			currentTime += Time.deltaTime;
			yield return null;
		}
		Destroy(gameObject);
	}
}
