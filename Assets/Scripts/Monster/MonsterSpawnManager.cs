using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonsterSpawner�� ���͸� ����ϱ� ���� Ŭ����
public class MonsterSpawnManager : MonoBehaviour
{
	// ������ ������ �����հ� �������� ����ִ� ����ü
	[System.Serializable]
    public struct MonsterSet
	{
		public GameObject monsterPrefab;
		public int monsterCount;
	}

	// ���̺� ���� ������ ����Ϳ� ������ ���� ����ü
	[System.Serializable]
	public struct Wave
	{
		public MonsterSet[] monsters;
	}

	[SerializeField] Transform spawnersTransform;
	[SerializeField] Wave[] waves;
	[SerializeField] Timer timer;

	[SerializeField] int readyTime = 10;
	[SerializeField] int waveInterval;

	MonsterSpawner[] spawners;
	Stack<GameObject> monsterStack;

	// wave
	int index = 0;

	private void Start()
	{
		monsterStack = new Stack<GameObject>();
		List<MonsterSpawner> list = new();
		foreach (Transform gameObject in spawnersTransform)
		{
			list.Add(gameObject.GetComponent<MonsterSpawner>());
		}
		spawners = list.ToArray();
		PopupMessage.Instance.Show("���⸦ �����ϰ� ������ ����ϼ���.", 2.5f);
	}

	private void Update()
	{
		// ���� ���̺� �ð��� �Ǹ�
		if(waveInterval * index + readyTime < timer.time)
		{
			// ��� ���̺갡 ������ ���̺� ����
			if (waves.Length <= index)
			{
				return;
			}
			// ���� ���̺� ����
			SetWave();
			// ���͹� ������ ���̺� ����
			StartWave();					
			index++;
		}
	}

	// ���̺� ����
	private void SetWave()
	{
		// ���� ���̺긦 ������
		Wave wave = waves[index];
		// ���̺꿡 ��ϵ� MonsterSet�� ������
		foreach (MonsterSet set in wave.monsters)
		{
			// MonsterSet�� ��ϵ� ���͵��� ���� ���ÿ� �߰��Ѵ�
			for (int i = 0; i < set.monsterCount; i++)
			{ 
				monsterStack.Push(set.monsterPrefab);
			}
		}
		// MonsterSpawner �迭�� ���´�
		spawners = spawners.OrderBy((x)=> Random.value).ToArray();

		int spawnerCount = spawners.Length;
		int spawnerIndex = 0;

		// �̹� ���̺꿡 ��ϵ� ������ ����ŭ
		while (monsterStack.Count > 0)
		{
			//spawner�� �ϳ��� ��ȸ�ϸ鼭 ������ ���͸� ����Ѵ�
			spawners[spawnerIndex % spawnerCount].AddMonster(monsterStack.Pop());
			spawnerIndex++;
		}
	}

	// ���̺� ����
	private void StartWave()
	{
		// �˾��޽����� ���� ������ �˸���
		PopupMessage.Instance.Show("���Ͱ� �����Ǿ����ϴ�.\n������ �غ��ϼ���", 2f);

		// ���� ����
		foreach(MonsterSpawner spawner in spawners)
		{
			spawner.SpawnMonster();
		}
	}
}
