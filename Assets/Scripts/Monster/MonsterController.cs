using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 몬스터의 종류
public enum Monster
{
    DEMON,
    SPIDER,
    GHOST,
}

public class MonsterController : MonoBehaviourPun
{
    // 몬스터 ai를 위한 상태
    protected enum STATE
	{
        WAIT,
        MOVE,
        ATTACK,
	}

    protected virtual void Movement()
	{
	}

    protected virtual void Attack()
	{
	}

    protected virtual void OnAttack()
	{
	}

    protected virtual void Wait()
	{
	}

    public virtual void Init(Vector3 position, Transform target)
	{
	}
}
