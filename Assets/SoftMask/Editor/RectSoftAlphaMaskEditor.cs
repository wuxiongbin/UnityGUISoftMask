using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    [CustomEditor(typeof(RectSoftAlphaMask))]
    public class RectSoftAlphaMaskEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RectSoftAlphaMask mask = target as RectSoftAlphaMask;
            Vector4 softEdge = mask.SoftEdge;

            EditorGUILayout.LabelField("软裁剪边");
            GUI.changed = false;
            softEdge.x = EditorGUILayout.FloatField("左边", softEdge.x);
            softEdge.w = EditorGUILayout.FloatField("顶边", softEdge.w);
            softEdge.z = EditorGUILayout.FloatField("右边", softEdge.z);
            softEdge.y = EditorGUILayout.FloatField("底边", softEdge.y);

            mask.SoftEdge = softEdge;

            if (GUI.changed)
            {
                mask.Update();
            }
        }
    }
}