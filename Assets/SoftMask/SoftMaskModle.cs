using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace UI
{
    public class SoftMaskModle : MonoBehaviour
    {
        static List<Renderer> Renderers = new List<Renderer>();

        public class Data
        {
            public Renderer renderer;
            public Material[] srcMaterials; // 源材质
            public Material[] dstMaterials; // 拷贝的材质

            public void Restore()
            {
                renderer.sharedMaterials = srcMaterials;
            }

            public void Set(UI.RectSoftAlphaMask mask)
            {
                for (int i = 0; i < dstMaterials.Length; ++i)
                {
                    mask.UpdateMaterial(dstMaterials[i]);
                }

                renderer.sharedMaterials = dstMaterials;
            }
        }

        public List<Data> mDatas = new List<Data>();

        public void Set()
        {
            if (mTotalMateiral == 0)
                return;

            RectSoftAlphaMask mask = GetComponentInParent<RectSoftAlphaMask>();
            if (mask == null)
                return;

            for (int i = 0; i < mDatas.Count; ++i)
            {
                mDatas[i].Set(mask);
            }
        }

        public void Restore()
        {
            if (mTotalMateiral == 0)
                return;

            for (int i = 0; i < mDatas.Count; ++i)
            {
                mDatas[i].Restore();
            }
        }

        int mTotalMateiral = 0;

        void Start()
        {
            Renderers.Clear();
            gameObject.GetComponentsInChildren(true, Renderers);

            mDatas.Capacity = Renderers.Count;
            mTotalMateiral = 0;
            for (int i = 0; i < Renderers.Count; ++i)
            {
                Data d = new Data();
                d.renderer = Renderers[i];
                d.srcMaterials = Renderers[i].sharedMaterials;
                d.dstMaterials = new Material[d.srcMaterials.Length];

                for (int j = 0; j < d.dstMaterials.Length; ++j)
                {
                    Shader shader;
                    Material mat = d.srcMaterials[j];
                    if (mat != null && ((shader = mat.shader) != null))
                    {
                        if (!mat.shader.name.EndsWith(" Soft Mask"))
                        {
                            Shader s = Shader.Find(shader.name + " Soft Mask");
                            if (s != null)
                            {
                                mat = new Material(mat);
                                mat.shader = s;
                                ++mTotalMateiral;

                                d.dstMaterials[j] = mat;
                            }
                        }
                    }
                }

                mDatas.Add(d);
            }

            Set();
        }

        void Update()
        {
            Set();
        }

        void OnDisable()
        {
            Restore();
        }

        void OnEnable()
        {
            Set();
        }
    }
}