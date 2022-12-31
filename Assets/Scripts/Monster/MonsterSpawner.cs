using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 몬스터 생성 기지
public class MonsterSpawner : MonoBehaviourPun
{
	// 생성 주기
	[SerializeField] float spawnRate;

	// 생성 대기중인 몬스터 스택
	Stack<GameObject> monsterStack;

	new Transform transform;

	private void Start()
	{
		// 스택 생성
		monsterStack = new Stack<GameObject>();
		transform = base.transform;
	}

	// 몬스터 스택에 몬스터 등록함수
	public void AddMonster(GameObject monster)
	{
		monsterStack.Push(monster);
	}

	// 몬스터 생성
	public void SpawnMonster()
	{
		// 호스트가 몬스터 생성
		if (PhotonNetwork.IsMasterClient)
			StartCoroutine(ISpawnMonster());
	}

	// 몬스터 생성 코루틴
	IEnumerator ISpawnMonster()
	{
		WaitForSeconds wait = new WaitForSeconds(spawnRate);
		// 생성할 몬스터가 있을 때
		while (monsterStack.Count > 0)
		{
			// 몬스터 생성
			GameObject newMonster = PhotonNetwork.Instantiate(monsterStack.Pop().name, transform.position, Quaternion.identity);
			// 생성한 몬스터의 초기화를 위해 MonsterController를 찾는다
			MonsterController control = newMonster.GetComponent<MonsterController>();
			// 살아있는 플레이어가 있다면
			if (PlayerStatus.playerTransformList.Count > 0)
			{
				// 가장 가까운 플레이어를 타겟으로 설정
				Transform targetTransform = PlayerStatus.playerTransformList.OrderBy(x => Vector3.Distance(x.position, transform.position)).First();
				control.Init(transform.position, targetTransform);
			}
			yield return wait;
		}
		yield return null;
	}

}
