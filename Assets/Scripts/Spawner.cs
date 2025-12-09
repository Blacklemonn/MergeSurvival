    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    private int level;
    private float timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
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
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        GameObject enemy =GameManager.instance.poolManager.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    //왜 int로 정의하였는가?
    public int health;
    public float speed;
    public int bounty;
}