using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2f;
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int _money;
    [Header("# Game Object")]
    public PoolManager poolManager;
    public Player player;
    public Shop shop;
    public Storage storage;
    public InventoryManager inventory;
    public Result uiResult;
    public GameObject enemyCleaner;
    public GameObject itemTemp;
    public int gameLevel;
    //[HideInInspector]
    public bool isDragging;

    private void Awake()
    {
        instance = this;
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        _money = 0;

        player.gameObject.SetActive(true);
        shop.Select(playerId % 2);    //무기선택
        Resume();

        AudioManager.instance.PlayBGM(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    //게임 이겼을때 불러오셈
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBGM(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > 0.2f)
        {
            if (gameTime > maxGameTime)
            {
                shop.Show();
                inventory.MergePiecesList();
                gameTime = maxGameTime;
            }
        }
    }
    public void Stop()
    {
        isLive = false;

        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        gameTime = 0;
        Time.timeScale = 1;
    }

    public void GetMoney(int money)
    {
        if (!isLive)
            return;

        _money += money;
    }

    public bool UseMoney(int money,bool useMoney)
    {
        //아이템을 살 수 있을경우 True
        if ((0 > _money - money))
        {
            return false;
        }
        else
        {
            if(useMoney)
            {
                _money -= money;
                return true;
            }
            else
            {
                return true;
            }
        }
    }

    public void LvlUp()
    {
        gameLevel++;
        if (maxGameTime < 60)
            maxGameTime += 5;
        //체력 초기화
        health = maxHealth;
    }
}
