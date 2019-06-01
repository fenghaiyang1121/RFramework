using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectManager : Singleton<ObjectManager>
{
    //对象池节点
    public Transform RecyclePoolTrs;
    //场景节点
    public Transform SceneTrs;
    //对象池
    protected Dictionary<uint, List<ResouceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResouceObj>>();
    //暂存ResObj的Dic
    protected Dictionary<int, ResouceObj> m_ResouceObjDic = new Dictionary<int, ResouceObj>();
    //ReourceObj的类对象池
    protected ClassObjectPool<ResouceObj> m_ResourceObjClassPool = null;
    //根据异步的guid储存ResourceObj,来判断是否正在异步加载
    protected Dictionary<long, ResouceObj> m_AsyncResObjs = new Dictionary<long, ResouceObj>();
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="recycleTrs">回收节点</param>
    /// <param name="sceneTrs">场景默认节点</param>
    public void Init(Transform recycleTrs,Transform sceneTrs)
    {
        m_ResourceObjClassPool = GetOrCreatClassPool<ResouceObj>(1000);
        RecyclePoolTrs = recycleTrs;
        SceneTrs = sceneTrs;
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void ClearCache()
    {
        List<uint> tempList = new List<uint>();
        foreach (uint key in m_ObjectPoolDic.Keys)
        {
            List<ResouceObj> st = m_ObjectPoolDic[key];
            for (int i = st.Count - 1; i >= 0; i--)
            {
                ResouceObj resObj = st[i];
                if (!System.Object.ReferenceEquals(resObj.m_CloneObj, null) && resObj.m_bClear)
                {
                    GameObject.Destroy(resObj.m_CloneObj);
                    m_ResouceObjDic.Remove(resObj.m_CloneObj.GetInstanceID());
                    resObj.Reset();
                    m_ResourceObjClassPool.Recycle(resObj);
                    st.Remove(resObj);
                }
            }

            if (st.Count <= 0)
            {
                tempList.Add(key);
            }
        }

        for(int i = 0; i < tempList.Count; i++)
        {
            uint temp = tempList[i];
            if (m_ObjectPoolDic.ContainsKey(temp))
            {
                m_ObjectPoolDic.Remove(temp);
            }
        }
        tempList.Clear();
    }

    /// <summary>
    /// 清除某个资源在对象池中所有的对象
    /// </summary>
    /// <param name="crc"></param>
    public void ClearPoolObject(uint crc)
    {
        List<ResouceObj> st = null;
        if (!m_ObjectPoolDic.TryGetValue(crc, out st) || st == null)
            return;

        for (int i = st.Count - 1; i >= 0; i--)
        {
            ResouceObj resObj = st[i];
            if (resObj.m_bClear)
            {
                st.Remove(resObj);
                int tempID = resObj.m_CloneObj.GetInstanceID();
                GameObject.Destroy(resObj.m_CloneObj);
                resObj.Reset();
                m_ResouceObjDic.Remove(tempID);
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }

        if (st.Count <= 0)
        {
            m_ObjectPoolDic.Remove(crc);
        }
    }

    /// <summary>
    /// 根据实例化对象直接获取离线数据
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <returns></returns>
    public OfflineData FindOfflineData(GameObject obj)
    {
        OfflineData data = null;
        ResouceObj resObj = null;
        m_ResouceObjDic.TryGetValue(obj.GetInstanceID(), out resObj);
        if (resObj != null)
        {
            data = resObj.m_OfflineData;
        }

        return data;
    }

    /// <summary>
    /// 取对象池去对象
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    protected ResouceObj GetObjectFromPool(uint crc)
    {
        List<ResouceObj> st = null;
        if (m_ObjectPoolDic.TryGetValue(crc, out st) && st != null && st.Count > 0)
        {
            ResourceManager.Instance.IncreaseResouceRef(crc);
            ResouceObj resObj = st[0];
            st.RemoveAt(0);
            GameObject obj = resObj.m_CloneObj;
            if (!System.Object.ReferenceEquals(obj, null))
            {
                if (!System.Object.ReferenceEquals(resObj.m_OfflineData, null))
                {
                    resObj.m_OfflineData.ResetProp();
                }
                resObj.m_Already = false;

#if UNITY_EDITOR
                if (obj.name.EndsWith("(Recycle)"))
                {
                    obj.name = obj.name.Replace("(Recycle)", "");
                }
#endif
            }
            return resObj;
        }
        return null;
    }

    /// <summary>
    /// 取消异步加载
    /// </summary>
    /// <param name="guid"></param>
    public void CancleLoad(long guid)
    {
        ResouceObj resObj = null;
        if (m_AsyncResObjs.TryGetValue(guid, out resObj) && ResourceManager.Instance.CancleLoad(resObj))
        {
            m_AsyncResObjs.Remove(guid);
            resObj.Reset();
            m_ResourceObjClassPool.Recycle(resObj);
        }
    }

    /// <summary>
    /// 是否正在异步加载
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public bool IsingAsyncLoad(long guid)
    {
        return m_AsyncResObjs[guid] != null;
    }

    /// <summary>
    /// 该对象是否是对象池创建的
    /// </summary>
    /// <returns></returns>
    public bool IsObjectManagerCreat(GameObject obj)
    {
        ResouceObj resObj = m_ResouceObjDic[obj.GetInstanceID()];
        return resObj == null ? false : true;
    }

    /// <summary>
    /// 预加载Gamobject
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="count">预加载个数</param>
    /// <param name="clear">跳场景是否清除</param>
    public void PreloadGameObject(string path, int count = 1, bool clear = false)
    {
        List<GameObject> tempGameObjectList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = InstantiateObject(path, false, bClear: clear);
            tempGameObjectList.Add(obj);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = tempGameObjectList[i];
            ReleaseObject(obj);
            obj = null;
        }

        tempGameObjectList.Clear();
    }

    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bClear"></param>
    /// <returns></returns>
    public GameObject InstantiateObject(string path, bool setSceneObj = false, bool bClear = true)
    {
        uint crc = Crc32.GetCrc32(path);
        ResouceObj resouceObj = GetObjectFromPool(crc);
        if (resouceObj == null)
        {
            resouceObj = m_ResourceObjClassPool.Spawn(true);
            resouceObj.m_Crc = crc;
            resouceObj.m_bClear = bClear;
            //ResouceManager提供加载方法
            resouceObj = ResourceManager.Instance.LoadResource(path, resouceObj);

            if (resouceObj.m_ResItem.m_Obj != null)
            {
                resouceObj.m_CloneObj = GameObject.Instantiate(resouceObj.m_ResItem.m_Obj) as GameObject;
                resouceObj.m_OfflineData = resouceObj.m_CloneObj.GetComponent<OfflineData>();
            }
        }

        if (setSceneObj)
        {
            resouceObj.m_CloneObj.transform.SetParent(SceneTrs, false);
        }

        int tempID = resouceObj.m_CloneObj.GetInstanceID();
        if (!m_ResouceObjDic.ContainsKey(tempID))
        {
            m_ResouceObjDic.Add(tempID, resouceObj);
        }

        return resouceObj.m_CloneObj;
    }

    /// <summary>
    /// 异步对象加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fealFinish"></param>
    /// <param name="priority"></param>
    /// <param name="setSceneObject"></param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="bClear"></param>
    public long InstantiateObjectAsync(string path , OnAsyncObjFinish dealFinish, LoadResPriority priority, bool setSceneObject = false, object param1 = null,object param2 = null, object param3 = null, bool bClear = true)
    {
        if (string.IsNullOrEmpty(path))
        {
            return 0;
        }

        uint crc = Crc32.GetCrc32(path);
        ResouceObj resObj = GetObjectFromPool(crc);
        if (resObj != null)
        {
            if (setSceneObject)
            {
                resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
            }

            if (dealFinish != null)
            {
                dealFinish(path, resObj.m_CloneObj, param1, param2, param3);
            }

            return resObj.m_Guid;
        }
        long guid = ResourceManager.Instance.CreatGuid();
        resObj = m_ResourceObjClassPool.Spawn(true);
        resObj.m_Crc = crc;
        resObj.m_SetSceneParent = setSceneObject;
        resObj.m_bClear = bClear;
        resObj.m_DealFinish = dealFinish;
        resObj.m_Param1 = param1;
        resObj.m_Param2 = param2;
        resObj.m_Param3 = param3;
        //调用ResouceManager的异步加载接口
        ResourceManager.Instance.AsyncLoadResource(path, resObj, OnLoadResouceObjFinish, priority);
        return guid;
    }

    /// <summary>
    /// 资源加载完成回调
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="resObj">中间类</param>
    /// <param name="param1">参数1</param>
    /// <param name="param2">参数2</param>
    /// <param name="param3">参数3</param>
    void OnLoadResouceObjFinish(string path, ResouceObj resObj, object param1 = null, object param2 = null, object param3 = null)
    {
        if (resObj == null)
            return;

        if (resObj.m_ResItem.m_Obj == null)
        {
#if UNITY_EDITOR
            Debug.LogError("异步资源加载的资源为空：" + path);
#endif
        }
        else
        {
            resObj.m_CloneObj = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
            resObj.m_OfflineData = resObj.m_CloneObj.GetComponent<OfflineData>();
        }

        //加载完成就从正在加载的异步中移除
        if (m_AsyncResObjs.ContainsKey(resObj.m_Guid))
        {
            m_AsyncResObjs.Remove(resObj.m_Guid);
        }

        if (resObj.m_CloneObj != null && resObj.m_SetSceneParent)
        {
            resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
        }

        if (resObj.m_DealFinish != null)
        {
            int tempID = resObj.m_CloneObj.GetInstanceID();
            if (!m_ResouceObjDic.ContainsKey(tempID))
            {
                m_ResouceObjDic.Add(tempID, resObj);
            }

            resObj.m_DealFinish(path, resObj.m_CloneObj, resObj.m_Param1, resObj.m_Param2, resObj.m_Param3);
        }
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="maxCacheCount"></param>
    /// <param name="destoryCache"></param>
    /// <param name="recycleParent"></param>
    public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destoryCache = false, bool recycleParent = true)
    {
        if (obj == null)
            return;

        ResouceObj resObj = null;
        int tempID = obj.GetInstanceID();
        if (!m_ResouceObjDic.TryGetValue(tempID, out resObj))
        {
            Debug.Log(obj.name + "对象不是ObjectManager创建的！");
            return;
        }

        if (resObj == null)
        {
            Debug.LogError("缓存的ResouceObj为空！");
            return;
        }

        if (resObj.m_Already)
        {
            Debug.LogError("该对象已经放回对象池了，检测自己是否情况引用!");
            return;
        }

#if UNITY_EDITOR
        obj.name += "(Recycle)";
#endif

        List<ResouceObj> st = null;
        if (maxCacheCount == 0)
        {
            m_ResouceObjDic.Remove(tempID);
            ResourceManager.Instance.ReleaseResouce(resObj, destoryCache);
            resObj.Reset();
            m_ResourceObjClassPool.Recycle(resObj);
        }
        else//回收到对象池
        {
            if (!m_ObjectPoolDic.TryGetValue(resObj.m_Crc, out st) || st == null)
            {
                st = new List<ResouceObj>();
                m_ObjectPoolDic.Add(resObj.m_Crc, st);
            }

            if (resObj.m_CloneObj)
            {
                if (recycleParent)
                {
                    resObj.m_CloneObj.transform.SetParent(RecyclePoolTrs);
                }
                else
                {
                    resObj.m_CloneObj.SetActive(false);
                }
            }

            if (maxCacheCount < 0 || st.Count < maxCacheCount)
            {
                st.Add(resObj);
                resObj.m_Already = true;
                //ResourceManager做一个引用计数
                ResourceManager.Instance.DecreaseResoucerRef(resObj);
            }
            else
            {
                m_ResouceObjDic.Remove(tempID);
                ResourceManager.Instance.ReleaseResouce(resObj, destoryCache);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }
    }

    #region 类对象池的使用
    protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

    /// <summary>
    /// 创建类对象池，创建完成以后外面可以保存ClassObjectPool<T>,然后调用Spawn和Recycle来创建和回收类对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxcount"></param>
    /// <returns></returns>
    public ClassObjectPool<T> GetOrCreatClassPool<T>(int maxcount) where T : class, new()
    {
        Type type = typeof(T);
        object outObj = null;
        if (!m_ClassPoolDic.TryGetValue(type, out outObj) || outObj == null)
        {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxcount);
            m_ClassPoolDic.Add(type, newPool);
            return newPool;
        }

        return outObj as ClassObjectPool<T>;
    }
    #endregion
}
