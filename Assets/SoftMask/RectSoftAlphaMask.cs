using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/RectSoftAlphaMask", 16)]
    [ExecuteInEditMode]
	public class RectSoftAlphaMask : MonoBehaviour
	{
        [HideInInspector]
        public Vector4 SoftEdge = new Vector4(10, 10, 10, 10); // 软边

        static List<AlphaMaskMaterial> s_AlphaMaskMaterial = new List<AlphaMaskMaterial>();

        void OnDisable()
        {
            s_AlphaMaskMaterial.Clear();
            gameObject.GetComponentsInChildren(s_AlphaMaskMaterial);
            for (int i = 0; i < s_AlphaMaskMaterial.Count; ++i)
                s_AlphaMaskMaterial[i].SetMaterialDirty();
            s_AlphaMaskMaterial.Clear();
        }

        void OnEnable()
        {
            Set();
        }

        public Rect MaskRect
        {
            get
            {
                Rect rect = rectTransform.rect;
                rect.xMin -= SoftEdge.x;
                rect.xMax -= SoftEdge.z;
                rect.yMin -= SoftEdge.y;
                rect.yMax -= SoftEdge.w;
                return rect;
            }
        }

        RectTransform rectTransform;

        [SerializeField]
        RectSoftAlphaMask parentMask;

        static List<Graphic> s_graphics = new List<Graphic>();
        
        void Set()
        {
            s_graphics.Clear();
            GetComponentsInChildren(s_graphics);
            GameObject go = gameObject;
            for (int i = 0; i < s_graphics.Count; ++i)
            {
                if (go == s_graphics[i].gameObject)
                    continue;

                AlphaMaskMaterial amm = s_graphics[i].GetComponent<AlphaMaskMaterial>();
                if (amm == null)
                {
                    amm = s_graphics[i].gameObject.AddComponent<AlphaMaskMaterial>();
                    amm.rectParent = this;
                }

                amm.SetMaterialDirty();
            }

            s_graphics.Clear();
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            currentMaterial = null;
        }

        [SerializeField]
        Material currentMaterial;

        [SerializeField]
        Material greyMaterial;

        static Vector3[] worldCorners;

        public void Update()
        {
            if (currentMaterial == null && greyMaterial == null)
                return;

            if (!rectTransform.hasChanged)
                return;

            if (currentMaterial != null)
                UpdateMaterial(currentMaterial);
            if (greyMaterial != null)
                UpdateMaterial(greyMaterial);
        }

        Rect GetSelfMaskRect()
        {
            float xMin, xMax, yMin, yMax;

            worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            xMin = Mathf.Min(worldCorners[0].x, worldCorners[1].x);
            xMin = Mathf.Min(xMin, worldCorners[2].x);
            xMin = Mathf.Min(xMin, worldCorners[3].x);

            yMin = Mathf.Min(worldCorners[0].y, worldCorners[1].y);
            yMin = Mathf.Min(yMin, worldCorners[2].y);
            yMin = Mathf.Min(yMin, worldCorners[3].y);

            xMax = Mathf.Max(worldCorners[0].x, worldCorners[1].x);
            xMax = Mathf.Max(xMax, worldCorners[2].x);
            xMax = Mathf.Max(xMax, worldCorners[3].x);

            yMax = Mathf.Max(worldCorners[0].y, worldCorners[1].y);
            yMax = Mathf.Max(yMax, worldCorners[2].y);
            yMax = Mathf.Max(yMax, worldCorners[3].y);

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public Rect GetMaskRect()
        {
            if (parentMask == null)
                return GetSelfMaskRect();

            Rect rect = GetSelfMaskRect();
            Rect parentRect = parentMask.GetMaskRect();

            rect.xMin = Mathf.Max(rect.xMin, parentRect.xMin);
            rect.xMax = Mathf.Min(rect.xMax, parentRect.xMax);
            rect.yMin = Mathf.Max(rect.yMin, parentRect.yMin);
            rect.yMax = Mathf.Min(rect.yMax, parentRect.yMax);

            return rect;
        }

        public void UpdateMaterial(Material material)
        {
            Rect maskRect = GetMaskRect();

            // Pass the values to the shader
            material.SetVector("_Min", maskRect.min);
            material.SetVector("_Max", maskRect.max);

            Vector2 size = maskRect.size;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);

            Vector2 screenSize = rectTransform.rect.size;

            size.x /= screenSize.x;
            size.y /= screenSize.y;

            Vector4 softEdge = new Vector4(
                SoftEdge.x * size.x,
                SoftEdge.y * size.y,
                SoftEdge.z * size.x,
                SoftEdge.w * size.y);

            material.SetVector("_SoftEdge", softEdge);
        }

        public Material GetDefaultMaterial(Material baseMaterial)
        {
            if (baseMaterial == Canvas.GetDefaultCanvasMaterial())
            {
                if (currentMaterial == null)
                {
                    currentMaterial = new Material(Shader.Find("UI/Default Soft Mask"));
                }

                return currentMaterial;
            }

            if (baseMaterial.shader.name == "UI/Default Grey")
            {
                if (greyMaterial == null)
                {
                    greyMaterial = new Material(Shader.Find("UI/Default Grey Soft Mask"));
                }

                return greyMaterial;
            }

            return baseMaterial;
        }
	}
}