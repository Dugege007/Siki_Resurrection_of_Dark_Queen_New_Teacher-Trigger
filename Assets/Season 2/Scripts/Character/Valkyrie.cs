using UnityEngine;

//瓦格里
public class Valkyrie : Master
{
    private AudioClip refreshClip;
    private AudioClip refreshEnemyClip;
    private AudioClip beamClip;
    private AudioClip unLockClip;
    private AudioSource audioSource;

    private GameObject shadowBeamStartGO;
    private GameObject shadowBeamEndGO;
    private GameObject shadowBeamGO;
    private LineRenderer lr;
    private bool isStartBeamSkill;

    protected override void Start()
    {
        base.Start();
        animator = cbc.animators[(int)State.Valkyrie];
        refreshClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/Refresh");
        refreshEnemyClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/RefreshEnemyTarget");
        beamClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/GuidingSkill");
        unLockClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/UnLocked");
        audioSource = GetComponent<AudioSource>();

        shadowBeamStartGO = CharacterBaseController.DeepFindChild(transform, "ShadowBeamStart").gameObject;
        shadowBeamEndGO = CharacterBaseController.DeepFindChild(transform, "ShadowBeamImpact").gameObject;
        shadowBeamGO = CharacterBaseController.DeepFindChild(transform, "Shadow Beam").gameObject;
        lr = shadowBeamGO.GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {

    }

    private void Update()
    {
        if (isStartBeamSkill)
        {
            UpdateBeam();
        }
    }

    private void Refresh()
    {
        CharacterBaseController targetCBC = cbc.targetTransCBC;
        if (targetCBC)
        {
            if (targetCBC.gameObject.layer == gameObject.layer && targetCBC.isDead)
            {
                audioSource.PlayOneShot(refreshClip);
                //AudioSource.PlayClipAtPoint(refreshClip, transform.position);
                targetCBC.PlayReviveAnimation();
            }
            else
            {
                audioSource.PlayOneShot(refreshEnemyClip);
                //AudioSource.PlayClipAtPoint(refreshEnemyClip, transform.position);
            }
        }
        else
        {
            audioSource.PlayOneShot(refreshEnemyClip);
        }
    }

    private void PlayReviveEffect()
    {
        CharacterBaseController targetCBC = cbc.targetTransCBC;
        if (targetCBC)
        {
            if (targetCBC.gameObject.layer == gameObject.layer)
                targetCBC.PlayOrStopReviveEffect(true);
        }
    }

    private void ShowOrHideShadowBeamStart(int show)
    {
        CharacterBaseController targetCBC = cbc.targetTransCBC;
        if (targetCBC)
        {
            if (targetCBC.gameObject.layer == gameObject.layer && !targetCBC.isDead)
            {
                return;
            }
        }
        else
        {
            if (!cbc.targetTrans)
            {
                return;
            }
        }

        isStartBeamSkill = System.Convert.ToBoolean(show);
        shadowBeamStartGO.SetActive(isStartBeamSkill);
        shadowBeamEndGO.SetActive(isStartBeamSkill);
        shadowBeamGO.SetActive(isStartBeamSkill);

        if (!isStartBeamSkill)
        {
            if (targetCBC)
            {
                if (targetCBC.gameObject.layer != gameObject.layer && !targetCBC.isDead)
                {
                    targetCBC.TakeDamage(3, transform.position);
                }
            }
            else
            {
                audioSource.PlayOneShot(unLockClip);
                cbc.targetTrans.GetComponent<Cage>().UnLockValkyrie();
            }
        }
        else
        {
            audioSource.PlayOneShot(beamClip);
        }
    }

    private void UpdateBeam()
    {
        CharacterBaseController targetCBC = cbc.targetTransCBC;
        //是否是人物
        if (targetCBC)
        {
            if (targetCBC.gameObject.layer != gameObject.layer)
            {
                shadowBeamEndGO.transform.position = targetCBC.transform.position;
                lr.SetPosition(0, shadowBeamStartGO.transform.position);
                lr.SetPosition(1, shadowBeamEndGO.transform.position + Vector3.up);
                lr.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * 12, 0);
            }
        }
        //不是人物
        else
        {
            //对象是否是牢笼
            if (cbc.targetTrans)
            {
                shadowBeamEndGO.transform.position = cbc.targetTrans.position;
                lr.SetPosition(0, shadowBeamStartGO.transform.position);
                lr.SetPosition(1, shadowBeamEndGO.transform.position + Vector3.up * 0.75f);
                lr.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * 12, 0);
            }
        }
    }
}
