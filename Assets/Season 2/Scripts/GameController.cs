using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 创建人：Dugege
 * 功能说明：游戏控制
 * 创建时间：2023年2月10日00:25:53
 */

public class GameController : MonoBehaviour
{
    public GameObject toNextBossGO;
    private bool loadNext;
    private GameObject playerGO;

    private void Start()
    {
        playerGO = GameObject.Find("Player");
    }

    private void Update()
    {
        if (ToNextLevel() && !loadNext)
        {
            //通向下一关卡
            loadNext = true;
            int indexNum = SceneManager.GetActiveScene().buildIndex + 1;
            if (indexNum >= 5)
            {
                indexNum = 0;
            }
            SceneManager.LoadSceneAsync(indexNum);
        }
        if (!playerGO && !loadNext)
        {
            loadNext = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private bool ToNextLevel()
    {
        return toNextBossGO == null;
    }
}
