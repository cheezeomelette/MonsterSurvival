# MonsterSurvival

- 끊임없이 생성되는 몬스터로부터 10분간 생존하는 게임

## 📈 클래스 다이어그램

![%EC%8A%A4%ED%8F%AC%EB%84%88_%EC%BB%B4%ED%8F%AC%EB%84%8C%ED%8A%B8](https://user-images.githubusercontent.com/81815193/211207735-c12d2b30-62ce-4084-a2d7-f08594550c4c.png)
![MonsterServivalClass drawio](https://user-images.githubusercontent.com/81815193/211210550-7f937b19-d26c-4e49-9907-57aa0fd4d28e.png)



## 💡주요 기능


### 👾몬스터 생성

- 일정 시간별로 발생시킬 몬스터 웨이브를 유니티 에디터에서 쉽게 등록하기 위한 구조체이다.

```csharp
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
```

![%EC%8A%A4%ED%8F%AC%EB%84%88_%EC%BB%B4%ED%8F%AC%EB%84%8C%ED%8A%B8](https://user-images.githubusercontent.com/81815193/211207735-c12d2b30-62ce-4084-a2d7-f08594550c4c.png)

- 구조체를 언패킹하여 생성할 몬스터의 프리팹를 스택에 모두 넣어두고 MonsterSpawner에 분배하는 함수

```csharp
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
```

### 🎇총알 판정 방식

- 투사체 방식은 총알 객체를 생성해서 Rigidbody컴포넌트를 이용해 발사하고 총알이 충돌 한 물체의 피격함수를 실행한다.
- 총알은 오브젝트 풀링을 이용해 관리한다.

```csharp
// 격발 함수
public virtual void Shoot()
{
	if (isReloading || currentBulletCount <= 0)
		return;

	if (fireTime > weaponData.fireRate)
	{
		Vector3 dir;

		// 총알방향을 크로스헤어 중심방향으로 설정
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, float.MaxValue))
		{
			dir = (hit.point - bulletPivot.position).normalized;
			Debug.Log(hit.transform.name);
		}
		else
			dir = transform.forward;

		// 총알을 오브젝트풀에서 꺼냄
		Bullet bullet = BulletManager.Instance.GetPool();
		bullet.transform.position = bulletPivot.position;
		// 총알 격발
		bullet.Setup(weaponData.bulletSpeed, weaponData.power, dir, score.AddDamage);
		source.PlayOneShot(shootClip);

		fireTime = 0f;
		currentBulletCount -= 1;
		// 총알 ui업데이트
		if (currentBulletCount <= 0)
			Reloading();
		GunHUD.Instance.Shoot(currentBulletCount);
	}
}
```

- 히트스캔 방식은 관통이 가능하게 만들어서 크로스헤어의 중심방향으로 Ray를 쏴서 Ray에 맞은 모든 물체의 피격함수를 실행한다.
- 투사체 방식을 상속받아 Shoot 함수만 오버라이드 해서 사용했다.

```csharp
// 격발 함수
public override void Shoot()
{
	if (isReloading)
		return;

	// 공격속도가 지나면 
	if (fireTime > weaponData.fireRate)
	{
		RaycastHit[] hits;
		// Ray를 발사해 ray에 맞은 모든 충돌체를 가져옴
		hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, float.MaxValue);
		if(hits.Length > 0)
		{
			// 모든 충돌체에서
			foreach(RaycastHit hit in hits)
			{
				if (hit.collider.CompareTag("Player"))
					continue;
				// IDamageable을 가져와서 OnDamage 호출
				IDamageable enemy = hit.collider.GetComponent<IDamageable>();
				if(enemy!= null)
					enemy.OnDamage(weaponData.power);
			}
		}

		BulletLine bulletLine = Instantiate(line, bulletPivot.position, Quaternion.identity);
		if (hits.Length > 0)
			bulletLine.Shoot(hits[hits.Length - 1].point - bulletPivot.position);
		else
			bulletLine.Shoot(bulletPivot.forward);

		fireTime = 0f;
		currentBulletCount -= 1;
		if (currentBulletCount <= 0)
			StartCoroutine(Reload());
		GunHUD.Instance.Shoot(currentBulletCount);
		source.PlayOneShot(shootClip);
	}
}
```

### 🎯피격

- 피격함수는 방에 접속한 모든 클라이언트와 체력을 동기화 하고 함수의 중복호출을 막기위해 호스트만 체력을 감소시킨 후 체력을 동기화하는 함수를 RPC로 호출한다.
- RPC로 OnDamage함수를 다시 호출하는 이유는 호스트가 hp를 깎았을 때 hp가 0보다 작아지는 순간 사망처리가 실행되어야 하지만 호스트 이외의 클라이언트에게는 아직 체력이 동기화 되기 전이기 때문에 체력을 동기화 시킨 후 다시 함수를 호출해서 사망처리를 실행시킨다

```csharp
// 피격 함수
[PunRPC]
public void OnDamage(float damage)
{
	// 죽었다면 함수 실행하지 않음
	if (dead)
		return;

	if (PhotonNetwork.IsMasterClient)
	{
		// 호스트가 먼저 제력감소 적용
		hp -= damage;
		if (hp <= 0)
			hp = 0;
		
		// 호스트에서 클라이언트로 동기화
		photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, hp, dead);
		// 다른 클라이언트들도 OnDamage를 실행하도록 함
		photonView.RPC("OnDamage", RpcTarget.Others, damage);
	}

	// 자신의 플레이어 UI업데이트, 최종 점수  
	if (photonView.IsMine)
	{
		playerHud.GetDamaged();
		score.GetDamaged(damage);
		myStat.UpdateHp(hp, maxHp);
	}

	// 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
	if (hp <= 0)
	{
		dead = true;
		// 캐릭터 입력을 막음
		playerMovement.enabled = false;
		playerControl.enabled = false;
		StartCoroutine(IDead());
		// 최종 점수에 사망 카운트 추가
		if (photonView.IsMine)
			score.Die();
	}
}
```

```csharp
// hp를 동기화 하기 위한 함수
[PunRPC]
public void ApplyUpdatedHealth(float newHp, bool isDead)
{
	hp = newHp;
	dead = isDead;
}
```
