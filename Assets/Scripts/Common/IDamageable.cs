using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 피해를 받는 생물 모두에게 적용하기 위해 인터페이스로 만듬
public interface IDamageable
{
    void OnDamage(float damage);
}
