﻿using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace Colorful.RenderGraph
{
    [CustomEditor(typeof(RenderGraphView))]
    public class RenderGraphInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.TextArea("Bonnie Bonnie!");
            //base.OnInspectorGUI();
        }

        [OnOpenAsset(0)]
        public static bool OnOpenRenderGraph(int instanceID, int line)   
        {
            try
            {
                var path = AssetDatabase.GetAssetPath(instanceID);
                if (AssetDatabase.LoadAssetAtPath<RenderGraphData>(path) != null)
                {
                    RGWindowView.Create().Load(path);
                    return true;
                }
                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load : " + e.Message);
                return false;
            }
        }
    }
}
