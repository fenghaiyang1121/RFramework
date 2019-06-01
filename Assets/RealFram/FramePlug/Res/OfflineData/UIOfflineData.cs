using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOfflineData : OfflineData
{
    public Vector2[] m_AnchorMax;
    public Vector2[] m_AnchorMin;
    public Vector2[] m_Pivot;
    public Vector2[] m_SizeDelta;
    public Vector3[] m_AnchoredPos;
    public ParticleSystem[] m_Particle;

    public override void ResetProp()
    {
        int allPointCount = m_AllPoint.Length;
        for (int i = 0; i > allPointCount; i++)
        {
            RectTransform tempTrs = m_AllPoint[i] as RectTransform;
            if (tempTrs != null)
            {
                tempTrs.localPosition = m_Pos[i];
                tempTrs.localRotation = m_Rot[i];
                tempTrs.localScale = m_Scale[i];
                tempTrs.anchorMax = m_AnchorMax[i];
                tempTrs.anchorMin = m_AnchorMin[i];
                tempTrs.pivot = m_Pivot[i];
                tempTrs.sizeDelta = m_SizeDelta[i];
                tempTrs.anchoredPosition3D = m_AnchoredPos[i];
            }

            if (m_AllPointActive[i])
            {
                if (!tempTrs.gameObject.activeSelf)
                {
                    tempTrs.gameObject.SetActive(true);
                }
            }
            else
            {
                if (tempTrs.gameObject.activeSelf)
                {
                    tempTrs.gameObject.SetActive(false);
                }
            }

            if (tempTrs.childCount > m_AllPointChildCount[i])
            {
                int childCount = tempTrs.childCount;
                for (int j = m_AllPointChildCount[i]; j < childCount; j++)
                {
                    GameObject tempObj = tempTrs.GetChild(j).gameObject;
                    if (!ObjectManager.Instance.IsObjectManagerCreat(tempObj))
                    {
                        GameObject.Destroy(tempObj);
                    }
                }
            }
        }

        int particleCount = m_Particle.Length;
        for (int i = 0; i < particleCount; i++)
        {
            m_Particle[i].Clear(true);
            m_Particle[i].Play();
        }
    }

    public override void BindData()
    {
        Transform[] allTrs = gameObject.GetComponentsInChildren<Transform>(true);
        int allTrsCount = allTrs.Length;
        for (int i = 0; i < allTrsCount; i++)
        {
            if (!(allTrs[i] is RectTransform))
            {
                allTrs[i].gameObject.AddComponent<RectTransform>();
            }
        }

        m_AllPoint = gameObject.GetComponentsInChildren<RectTransform>(true);
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        int allPointCount = m_AllPoint.Length;
        m_AllPointChildCount = new int[allPointCount];
        m_AllPointActive = new bool[allPointCount];
        m_Pos = new Vector3[allPointCount];
        m_Rot = new Quaternion[allPointCount];
        m_Scale = new Vector3[allPointCount];
        m_Pivot = new Vector2[allPointCount];
        m_AnchorMax = new Vector2[allPointCount];
        m_AnchorMin = new Vector2[allPointCount];
        m_SizeDelta = new Vector2[allPointCount];
        m_AnchoredPos = new Vector3[allPointCount];
        for (int i = 0; i < allPointCount; i++)
        {
            RectTransform temp = m_AllPoint[i] as RectTransform;
            m_AllPointChildCount[i] = temp.childCount;
            m_AllPointActive[i] = temp.gameObject.activeSelf;
            m_Pos[i] = temp.localPosition;
            m_Rot[i] = temp.localRotation;
            m_Scale[i] = temp.localScale;
            m_Pivot[i] = temp.pivot;
            m_AnchorMax[i] = temp.anchorMax;
            m_AnchorMin[i] = temp.anchorMin;
            m_SizeDelta[i] = temp.sizeDelta;
            m_AnchoredPos[i] = temp.anchoredPosition3D;
        }
    }
}
