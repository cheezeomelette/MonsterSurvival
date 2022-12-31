using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonsterSpawner에 몬스터를 등록하기 위한 클래스
public class MonsterSpawnManager : MonoBehaviour
{
	// 생성할 몬스터의 프리팹과 마릿수를 담고있는 구조체
	[System.Serializable]
    public struct MonsterSet
	{
		public GameObject monsterPrefab;
		public int monsterCount;
	}

	// 웨이브 별로 생성할 몬수터와 마릿수 설정 구조체
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
		PopupMessage.Instance.Show("무기를 구입하고 전투를 대비하세요.", 2.5f);
	}

	private void Update()
	{
		// 다음 웨이브 시간이 되면
		if(waveInterval * index + readyTime < timer.time)
		{
			// 모든 웨이브가 끝나면 웨이브 멈춤
			if (waves.Length <= index)
			{
				return;
			}
			// 다음 웨이브 세팅
			SetWave();
			// 인터벌 지나면 웨이브 시작
			StartWave();					
			index++;
		}
	}

	// 웨이브 세팅
	private void SetWave()
	{
		// 현재 웨이브를 가져옴
		Wave wave = waves[index];
		// 웨이브에 등록된 MonsterSet을 가져옴
		foreach (MonsterSet set in wave.monsters)
		{
			// MonsterSet에 등록된 몬스터들을 몬스터 스택에 추가한다
			for (int i = 0; i < set.monsterCount; i++)
			{ 
				monsterStack.Push(set.monsterPrefab);
			}
		}
		// MonsterSpawner 배열을 섞는다
		spawners = spawners.OrderBy((x)=> Random.value).ToArray();

		int spawnerCount = spawners.Length;
		int spawnerIndex = 0;

		// 이번 웨이브에 등록된 몬스터의 수만큼
		while (monsterStack.Count > 0)
		{
			//spawner를 하나씩 순회하면서 생성할 몬스터를 등록한다
			spawners[spawnerIndex % spawnerCount].AddMonster(monsterStack.Pop());
			spawnerIndex++;
		}
	}

	// 웨이브 시작
	private void StartWave()
	{
		// 팝업메시지로 몬스터 생성을 알린다
		PopupMessage.Instance.Show("몬스터가 생성되었습니다.\n전투를 준비하세요", 2f);

		// 몬스터 생성
		foreach(MonsterSpawner spawner in spawners)
		{
			spawner.SpawnMonster();
		}
	}
}
