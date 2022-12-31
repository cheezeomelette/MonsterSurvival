using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    [SerializeField] new ParticleSystem particleSystem;


	private void Update()
	{
		if (particleSystem.isStopped)
			BulletEffectManager.Instance.ReturnPool(this);
	}
}
