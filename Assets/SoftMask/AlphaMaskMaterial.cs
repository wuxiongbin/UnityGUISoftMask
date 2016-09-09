using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
	public class AlphaMaskMaterial : MonoBehaviour, IMaterialModifier
    {
        public RectSoftAlphaMask rectParent;
        Graphic graphic_;

        Graphic graphic
        {
            get
            {
                if (graphic_ == null)
                    graphic_ = GetComponent<Graphic>();
                return graphic_;
            }
        }

        void Awake()
        {
            if (graphic == null)
                Destroy(this);
        }

        void OnEnable()
        {
           if (graphic != null)
                graphic.SetMaterialDirty();
        }

        void OnDisable()
        {
            if (graphic != null)
                graphic.SetMaterialDirty();
        }

        public void SetMaterialDirty()
        {
            if (graphic != null)
                graphic.SetMaterialDirty();
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!enabled)
                return baseMaterial;

            if (rectParent == null)
            {
                rectParent = GetComponentInParent<RectSoftAlphaMask>();
                if (rectParent == null)
                {
#if UNITY_EDITOR
                    if (Application.isEditor)
                        Object.DestroyImmediate(this);
                    else
                        Object.Destroy(this);
#else
                    Object.Destroy(this);
#endif
                    return baseMaterial;
                }
            }

            if (!rectParent.enabled)
                return baseMaterial;

            return rectParent.GetDefaultMaterial(baseMaterial);
        }
    }
}