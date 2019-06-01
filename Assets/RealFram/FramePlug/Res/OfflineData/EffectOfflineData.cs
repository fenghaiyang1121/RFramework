using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOfflineData : OfflineData
{
    public ParticleSystem[] m_Particle;
    public TrailRenderer[] m_TrailRe;

    public override void ResetProp()
    {
        base.ResetProp();
        foreach (ParticleSystem particle in m_Particle)
        {
            particle.Clear(true);
            particle.Play();
        }

        foreach (TrailRenderer trail in m_TrailRe)
        {
            trail.Clear();
        }
    }

    public override void BindData()
    {
        base.BindData();
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        m_TrailRe = gameObject.GetComponentsInChildren<TrailRenderer>(true);
    }
}
