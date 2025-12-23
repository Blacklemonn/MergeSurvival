    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData;

    private BoxCollider2D spawnArea;

    public int warningTime = 0;

    public GameObject warningPrefab;

    private Vector2 spawnPos;
    private int level;
    private float timer;

    private void Awake()
    {
        spawnArea = GetComponentInChildren<BoxCollider2D>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        if (timer > spawnData[level].spawnTime)
        {
            if (timer > 0.2f)
            {
                timer = 0;
                StartCoroutine(MarkSpawn());
            }
        }
    }
    
    //표식이 사라지고 몬스터 소환
    private void Spawn()
    {
        if (IsPlayerInSpawnArea(spawnPos))
        {
            //스폰 취소
            return;
        }

        SpawnEnemy();
    }

    private bool IsPlayerInSpawnArea(Vector2 pos)
    {
        float checkRadius = 0.5f; // 몬스터 크기 기준
        LayerMask playerLayer = LayerMask.GetMask("Player");

        return Physics2D.OverlapCircle(pos, checkRadius, playerLayer);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = GameManager.instance.poolManager.Get(0);
        enemy.transform.position = spawnPos;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }

    //박스 콜라이더 크기를 가져와서 그 안에서 스폰되게 지정
    private void GetRandomPositionInArea()
    {
        Vector2 center = spawnArea.bounds.center;
        Vector2 size = spawnArea.bounds.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        spawnPos = new Vector2(x, y);
    }

    private IEnumerator MarkSpawn()
    {
        GetRandomPositionInArea();

        GameObject warning = Instantiate(warningPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(warningTime);

        Spawn();

        Destroy(warning);
    }
    
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    public int bounty;
}