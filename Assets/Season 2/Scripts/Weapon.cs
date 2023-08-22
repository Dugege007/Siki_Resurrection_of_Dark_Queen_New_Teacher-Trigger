using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterBaseController owner;
    public GameObject effectGO;
    private GameObject bloodGO;
    private AudioClip[] projectleClips;
    private AudioClip weaponHitClip;
    private Collider col;

    public bool needDestroy;
    public float destroyTime;
    public int damageValue;
    private readonly bool isBigMuzzle;
    //是否隐藏碰撞器
    public bool ifHideCollider = true;

    private void Awake()
    {
        gameObject.tag = "Weapon";
        owner = transform.root.GetComponent<CharacterBaseController>();
        col=GetComponent<Collider>();
        bloodGO = Resources.Load<GameObject>("Prefabs/Blood");
        projectleClips = new AudioClip[2];
        projectleClips[0] = Resources.Load<AudioClip>("AudioClips/Master/Hit1");
        projectleClips[1] = Resources.Load<AudioClip>("AudioClips/Master/Hit2");
        weaponHitClip = Resources.Load<AudioClip>("AudioClips/WeaponHit");
    }

    private void OnEnable()
    {
        if (destroyTime > 0)
        {
            CancelInvoke();
            Invoke(nameof(DestroyWeapon), destroyTime);
        }
    }

    private void Start()
    {
        if (effectGO)
            PoolManager.Instance.InitPool(effectGO, 6);
        PoolManager.Instance.InitPool(bloodGO, 6);
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterBaseController cbc = other.GetComponent<CharacterBaseController>();
        if (cbc)
        {
            if (other.CompareTag("Character"))
            {
                Debug.Log("受到伤害");
                if (cbc != null)
                {
                    if (cbc.CanExecute() && owner)
                    {
                        int random = Random.Range(1, 5);
                        if (owner.currentState == State.Assassin)
                        {
                            cbc.BeExecuted(random, owner.transform);
                            owner.Execute(random, cbc);
                        }
                        else
                        {
                            HitTarget(cbc, other);
                        }
                    }
                    else//对玩家造成伤害
                    {
                        HitTarget(cbc, other);
                    }
                }
            }
        }
        if (!other.CompareTag("Collider"))
        { 
            if (needDestroy)
            {
                if (effectGO)
                {
                    PoolManager.Instance.SetPosAndRot(PoolManager.Instance.GetInstance<GameObject>(effectGO).transform, other.ClosestPoint(transform.position), Quaternion.identity);
                    if (isBigMuzzle)
                        AudioSource.PlayClipAtPoint(projectleClips[1], transform.position);
                    else
                        AudioSource.PlayClipAtPoint(projectleClips[0], transform.position);
                }
                DestroyWeapon();
            }
        }
    }

    private void HitTarget(CharacterBaseController cbc, Collider other)
    {
        if (bloodGO && cbc.canDecreaseHP)
        {
            PoolManager.Instance.SetPosAndRot(PoolManager.Instance.GetInstance<GameObject>(bloodGO).transform, other.ClosestPoint(transform.position), bloodGO.transform.rotation);
            if (!needDestroy)
            {
                AudioSource.PlayClipAtPoint(weaponHitClip, transform.position);
            }
        }
        if (cbc.isAI)
        {
            cbc.targetTransCBC = owner;
        }
        cbc.TakeDamage(damageValue, other.ClosestPoint(transform.position));
        if (ifHideCollider)
        {
            col.enabled = false;
        }
    }

    private void DestroyWeapon()
    {
        gameObject.SetActive(false);
    }
}
