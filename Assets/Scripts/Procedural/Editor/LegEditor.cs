using System.Reflection;
using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Procedural.Editor
{
    [CustomEditor(typeof(Leg)), CanEditMultipleObjects]
    public class LegEditor : UnityEditor.Editor
    {
        private Leg _target;
        
        private void OnEnable()
        {
            _target ??= target as Leg;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            
            var chainIKProperty = serializedObject.FindProperty("_chainIk");
            var chainIK = chainIKProperty.objectReferenceValue as ChainIKConstraint;
            
            if (chainIK == null) return;
            
            var dataProperty = chainIK.GetType().GetField("m_Data", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (dataProperty == null) return;
            
            var data = (ChainIKConstraintData) dataProperty.GetValue(chainIK);

            if (data.root != _target.transform)
                data.root = _target.transform;

            var tip = _target.transform.GetLastChild();

            if (data.tip != tip)
                data.tip = tip;

            if (!data.target)
            {
                var iKTarget = new GameObject(_target.gameObject.name + " IK Target").transform;
                iKTarget.position = data.tip.position;
                
                if (_target.transform.parent != null)
                {
                    iKTarget.SetParent(_target.transform.parent);
                    iKTarget.SetSiblingIndex(_target.transform.GetSiblingIndex());
                }

                data.target = iKTarget;
                _target.SetTarget(iKTarget);
            }
            
            dataProperty.SetValue(chainIK, data);
        }
    }
}