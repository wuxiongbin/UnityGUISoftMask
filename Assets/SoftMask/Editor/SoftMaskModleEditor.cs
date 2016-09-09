using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    [CustomEditor(typeof(SoftMaskModle), true)]
    public class SoftMaskModleEditor : Editor
    {
        Vector2 ScrollPosition;

        bool isFoldout = false;

        public override void OnInspectorGUI()
        {
            SoftMaskModle maskModle = target as SoftMaskModle;
            List<SoftMaskModle.Data> datas = maskModle.mDatas;

            isFoldout = EditorGUILayout.Foldout(isFoldout, "show");
            if (!isFoldout)
                return;

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            for (int i = 0; i < datas.Count; ++i)
            {
                EditorGUILayout.ObjectField("obj", datas[i].renderer, typeof(Renderer), true);
                EditorGUI.indentLevel++;
                SoftMaskModle.Data d = datas[i];
                for (int j = 0; j < d.dstMaterials.Length; ++j)
                {
                    Material src = d.srcMaterials[j];
                    Material dst = d.dstMaterials[j];
                    EditorGUILayout.ObjectField(string.Format("src id:{0}", src == null ? "" : src.GetInstanceID().ToString()), src, typeof(Material), true);
                    EditorGUILayout.ObjectField(string.Format("dst id:{0}", dst == null ? "" : dst.GetInstanceID().ToString()), dst, typeof(Material), true);
                }
                EditorGUI.indentLevel--;
            }

            GUILayout.EndScrollView();
        }
    }
}