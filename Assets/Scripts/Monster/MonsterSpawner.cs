using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ���� ���� ����
public class MonsterSpawner : MonoBehaviourPun
{
	// ���� �ֱ�
	[SerializeField] float spawnRate;

	// ���� ������� ���� ����
	Stack<GameObject> monsterStack;

	new Transform transform;

	private void Start()
	{
		// ���� ����
		monsterStack = new Stack<GameObject>();
		transform = base.transform;
	}

	// ���� ���ÿ� ���� ����Լ�
	public void AddMonster(GameObject monster)
	{
		monsterStack.Push(monster);
	}

	// ���� ����
	public void SpawnMonster()
	{
		// ȣ��Ʈ�� ���� ����
		if (PhotonNetwork.IsMasterClient)
			StartCoroutine(ISpawnMonster());
	}

	// ���� ���� �ڷ�ƾ
	IEnumerator ISpawnMonster()
	{
		WaitForSeconds wait = new WaitForSeconds(spawnRate);
		// ������ ���Ͱ� ���� ��
		while (monsterStack.Count > 0)
		{
			// ���� ����
			GameObject newMonster = PhotonNetwork.Instantiate(monsterStack.Pop().name, transform.position, Quaternion.identity);
			// ������ ������ �ʱ�ȭ�� ���� MonsterController�� ã�´�
			MonsterController control = newMonster.GetComponent<MonsterController>();
			// ����ִ� �÷��̾ �ִٸ�
			if (PlayerStatus.playerTransformList.Count > 0)
			{
				// ���� ����� �÷��̾ Ÿ������ ����
				Transform targetTransform = PlayerStatus.playerTransformList.OrderBy(x => Vector3.Distance(x.position, transform.position)).First();
				control.Init(transform.position, targetTransform);
			}
			yield return wait;
		}
		yield return null;
	}

}
