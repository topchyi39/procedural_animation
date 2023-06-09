using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Procedural.Editor
{
    [CustomEditor(typeof(ProceduralMoving))]
    public class ProceduralMovingEditor : UnityEditor.Editor
    {
        private ProceduralMoving _target;

        private void OnEnable()
        {
            _target ??= target as ProceduralMoving;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Validate"))
            {
                var method = _target.GetType().GetMethod("Validate", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(_target, null);
            }
        }
    }
}