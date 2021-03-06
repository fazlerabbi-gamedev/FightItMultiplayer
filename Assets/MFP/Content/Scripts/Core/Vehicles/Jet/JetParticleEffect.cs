using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(ParticleSystem))]
    public class JetParticleEffect : MonoBehaviour
    {
        // this script controls the jet's exhaust particle system, controlling the
        // size and colour based on the jet's current throttle value.
        public Color minColour; // The base colour for the effect to start at

        private AeroplaneController m_Jet; // The jet that the particle effect is attached to
        private ParticleSystem m_System; // The particle system that is being controlled
        private float m_OriginalStartSize; // The original starting size of the particle system
        private float m_OriginalLifetime; // The original lifetime of the particle system
        private Color m_OriginalStartColor; // The original starting colour of the particle system

        // Use this for initialization
        private void Start()
        {
            // get the aero plane from the object hierarchy
            m_Jet = FindAeroplaneParent();

            // get the particle system ( it will be on the object as we have a require component set up
            m_System = GetComponent<ParticleSystem>();

#if UNITY_5_5_OR_NEWER
            // set the original properties from the particle system
            var m_s = m_System.main;
            m_OriginalLifetime = m_s.startLifetime.constant;
            m_OriginalStartSize = m_s.startSize.constant;
            m_OriginalStartColor = m_s.startColor.color;
#else
            m_OriginalLifetime = m_System.startLifetime;
            m_OriginalStartSize = m_System.startSize;
            m_OriginalStartColor = m_System.startColor;
#endif
        }


        // Update is called once per frame
        private void Update()
        {
            // update the particle system based on the jets throttle
#if UNITY_5_5_OR_NEWER
            var m_s = m_System.main;
            m_s.startLifetime = Mathf.Lerp(0.0f, m_OriginalLifetime, m_Jet.Throttle);
            m_s.startSize = Mathf.Lerp(m_OriginalStartSize * .3f, m_OriginalStartSize, m_Jet.Throttle);
            m_s.startColor = Color.Lerp(minColour, m_OriginalStartColor, m_Jet.Throttle);
#else
            m_System.startLifetime = Mathf.Lerp(0.0f, m_OriginalLifetime, m_Jet.Throttle);
            m_System.startSize = Mathf.Lerp(m_OriginalStartSize * .3f, m_OriginalStartSize, m_Jet.Throttle);
            m_System.startColor = Color.Lerp(minColour, m_OriginalStartColor, m_Jet.Throttle);
#endif
        }


        private AeroplaneController FindAeroplaneParent()
        {
            // get reference to the object transform
            var t = transform;

            // traverse the object hierarchy upwards to find the AeroplaneController
            // (since this is placed on a child object)
            while (t != null)
            {
                var aero = t.GetComponent<AeroplaneController>();
                if (aero == null)
                {
                    // try next parent
                    t = t.parent;
                }
                else
                {
                    return aero;
                }
            }

            // controller not found!
            throw new Exception(" AeroplaneContoller not found in object hierarchy");
        }
    }
}
