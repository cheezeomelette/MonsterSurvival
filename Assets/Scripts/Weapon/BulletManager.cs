using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private static BulletManager instance;
	public static BulletManager Instance => instance;

    [SerializeField] Bullet prefab;
	[SerializeField] int initCount;

	private Stack<Bullet> pool;
	Transform poolTransform;

	private void Awake()
	{
		instance = this;

		pool = new Stack<Bullet>();

		poolTransform = new GameObject("BulletPool").transform;
		poolTransform.SetParent(transform);
		poolTransform.gameObject.SetActive(false);

		for (int i = 0; i < initCount; i++)
		{
			CreateBullet();
		}
	}


	private void CreateBullet()
	{
		Bullet bullet = Instantiate(prefab, poolTransform);
		pool.Push(bullet);
	}

	public Bullet GetPool()
	{
		if (pool.Count <= 0)
			CreateBullet();
		Bullet bullet = pool.Pop();
		bullet.transform.SetParent(null);
		return bullet;
	}

	public void ReturnPool(Bullet bullet)
	{
		bullet.transform.SetParent(poolTransform);
		pool.Push(bullet);
	}
}
