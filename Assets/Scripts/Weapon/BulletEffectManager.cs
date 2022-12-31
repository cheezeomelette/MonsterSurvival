using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffectManager : MonoBehaviour
{
	private static BulletEffectManager instance;
	public static BulletEffectManager Instance => instance;

	[SerializeField] BulletEffect prefab;
	[SerializeField] int initCount;

	private Stack<BulletEffect> pool;
	Transform poolTransform;

	private void Awake()
	{
		instance = this;

		pool = new Stack<BulletEffect>();

		poolTransform = new GameObject("BulletEffectPool").transform;
		poolTransform.SetParent(transform);
		poolTransform.gameObject.SetActive(false);

		for (int i = 0; i < initCount; i++)
		{
			CreateBullet();
		}
	}


	private void CreateBullet()
	{
		BulletEffect bulletEffect = Instantiate(prefab, poolTransform);
		pool.Push(bulletEffect);
	}

	public BulletEffect GetPool()
	{
		if (pool.Count <= 0)
			CreateBullet();
		BulletEffect bulletEffect = pool.Pop();
		bulletEffect.transform.SetParent(null);
		return bulletEffect;
	}

	public void ReturnPool(BulletEffect bulletEffect)
	{
		bulletEffect.transform.SetParent(poolTransform);
		pool.Push(bulletEffect);
	}
}
