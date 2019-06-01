using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassObjectPool<T> where T :class, new()
{
    //池
    protected Stack<T> m_Pool = new Stack<T>();
    //最大对象个数，<=0表示不限个数
    protected int m_MaxCount = 0;
    //没有回收的对象个数
    protected int m_NoRecycleCount = 0;

    public ClassObjectPool(int maxcount)
    {
        m_MaxCount = maxcount;
        for (int i = 0; i < maxcount; i++)
        {
            m_Pool.Push(new T());
        }
    }

    /// <summary>
    /// 从池里面取类对象
    /// </summary>
    /// <param name="creatIfPoolEmpty">如果为空是否new出来</param>
    /// <returns></returns>
    public T Spawn(bool creatIfPoolEmpty)
    {
        if (m_Pool.Count > 0)
        {
            T rtn = m_Pool.Pop();
            if (rtn == null)
            {
                if (creatIfPoolEmpty)
                {
                    rtn = new T();
                }
            }
            m_NoRecycleCount++;
            return rtn;
        }
        else
        {
            if (creatIfPoolEmpty)
            {
                T rtn = new T();
                m_NoRecycleCount++;
                return rtn;
            }
        }

        return null;
    }

    /// <summary>
    /// 回收类对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Recycle(T obj)
    {
        if (obj == null)
            return false;

        m_NoRecycleCount--;

        if (m_Pool.Count >= m_MaxCount && m_MaxCount > 0)
        {
            obj = null;
            return false;
        }

        m_Pool.Push(obj);
        return true;
    }
}
