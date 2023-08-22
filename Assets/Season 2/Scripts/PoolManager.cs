using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：Dugege
 * 功能说明：对象池管理
 * 创建时间：2023年1月28日17:40:59
 */

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    private Dictionary<Object, Queue<Object>> poolsDict;

    private void Awake()
    {
        Instance = this;
        poolsDict = new Dictionary<Object, Queue<Object>>();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab">预制体游戏物体</param>
    /// <param name="size">对象数量</param>
    public void InitPool(Object prefab, int size)
    {
        if (poolsDict.ContainsKey(prefab))
        {
            return;
        }
        Queue<Object> queue = new Queue<Object>();
        for (int i = 0; i < size; i++)
        {
            Object go = Instantiate(prefab);
            CreateGameObjectAndSetActive(go, false);
            //入队
            queue.Enqueue(go);
        }
        poolsDict[prefab] = queue;
    }

    private void CreateGameObjectAndSetActive(Object obj, bool isActive)
    {
        GameObject itemGO = null;
        if (obj is Component)
        {
            Component component = obj as Component;
            itemGO = component.gameObject;
        }
        else
        {
            itemGO = obj as GameObject;
        }
        itemGO.transform.SetParent(transform);
        itemGO.SetActive(isActive);
    }

    public T GetInstance<T>(Object prefab) where T : Object
    {
        Queue<Object> queue;
        if (poolsDict.TryGetValue(prefab, out queue))
        {
            Object obj;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab);
            }
            CreateGameObjectAndSetActive(obj, true);
            queue.Enqueue(obj);
            return obj as T;
        }
        Debug.Log("还没有当前类型的资源池被实例化");
        return null;
    }

    public void SetPosAndRot(Transform targetTrans,Vector3 targetPos,Quaternion targetRot)
    {
        targetTrans.position = targetPos;
        targetTrans.rotation = targetRot;
    }
}
