using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //무기 ID
    public int id;
    //프리펩 ID
    public int prefabId;
    //데미지
    public float damage;
    //개수
    public int count;
    //속도
    public float speed;

    //private List<GameObject> listBullet;
    private float timer;

    private Player player;

    private PoolManager poolManager;

    private void Awake()
    {
        player = GameManager.instance.player;
        poolManager = GameManager.instance.poolManager;
        //listBullet = new List<GameObject>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    Fire();
                }
                break;
        }
    }

    public void CountUp()
    {
        count += 1;

        if (id == 0)
            Place();

        //player.BroadcastMessage("ApplyWeapon", SendMessageOptions.DontRequireReceiver);
    }
    public void CountDown()
    {
        count -= 1;

        if (id == 0)
        {
            poolManager.Deactive(prefabId, count);

            Place();
        }

        //player.BroadcastMessage("ApplyWeapon", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int index = 0; index < GameManager.instance.poolManager.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.poolManager.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                Place();
                break;
            case 5:
                speed = 0.9f * Character.WeaponSpeed;
                break;
            default:
                speed = 0.3f * Character.WeaponSpeed;
                break;
        }

        //Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    private void Place()    //플레이어 기준 돌아가는 무기 로직
    {
        //자식의 갯수를 가져와서 count의 갯수만큼 활성화
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
                
                bullet.gameObject.SetActive (true);
            }
            else
            {
                bullet = GameManager.instance.poolManager.Get(prefabId).transform;
            }

            bullet.parent = transform;

            //위치 초기화
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity per
        }
    }

    void Fire()         //플레이어로부터 날아가는 로직
    {
        if (!player.scanner.nearstTarget)
            return;

        Vector3 targetPos = player.scanner.nearstTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        for (int i = id == 5 ? 0 : 9; i < 10; i++)
        {
            Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
            float randX = id == 5 ? Random.Range(-0.1f, 0.1f) : 0;
            float randY = id == 5 ? Random.Range(-0.1f, 0.1f) : 0;
            if (randX + dir.x > 1 || randX + dir.x < -1)
                randX = -randX;
            if (randY + dir.y> 1 || randY + dir.y < -1)
                randY = -randY;
            bullet.position = transform.position;
            dir = new Vector3(dir.x + randX, dir.y + randY + dir.z);
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
