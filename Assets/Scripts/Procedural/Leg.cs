using System;
using SecondOrderLinear;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace Procedural
{
    [Serializable]
    public class LegRaycastSettings
    {
        public float yOffset;
        public float raycastLength;
    }
    
    [RequireComponent(typeof(ChainIKConstraint))]
    public class Leg : MonoBehaviour
    {
        [SerializeField] private Leg oppositeLeg;
        [SerializeField, Range(0f, 2f)] private float maxDistance = 0.5f;
        [SerializeField] private float tolerance = 0.001f;
        [SerializeField] private float f = 1f;
        [SerializeField] private float z = 1f;
        [SerializeField] private float r = 0f;
        [SerializeField, Range(0.01f, 1f)] private float yMultiplier = 0.2f;
        
        
        [SerializeField] private LegRaycastSettings raycastSettings;

        [SerializeField, HideInInspector] private ChainIKConstraint _chainIk;
        [SerializeField, HideInInspector] private Transform _target;
        [SerializeField, HideInInspector] private Transform _root;
        [SerializeField, HideInInspector] private Vector3 _attachedBodyOffset;
        
        [SerializeField] private bool debug;

        private Transform _attachedToBodyTarget;
        private Vector3 _targetPosition;
        private bool _inPosition;

        private VectorLinear _vectorLinear;

        // public bool Moving => !_inPosition;
        
        private void OnValidate()
        {
            _chainIk ??= GetComponent<ChainIKConstraint>();
        }

        private void Awake()
        {
            _attachedToBodyTarget = Instantiate(_target, _root);
            _attachedToBodyTarget.position = _attachedToBodyTarget.position + _attachedBodyOffset;
            _vectorLinear = new VectorLinear(f, z, r, _target.position);
        }

        private void FixedUpdate()
        {
            var startPosition = _attachedToBodyTarget.position + _attachedToBodyTarget.up * raycastSettings.yOffset;
            var ray = new Ray(startPosition, -_attachedToBodyTarget.up);

            if (Physics.Raycast(ray, out var hit, raycastSettings.raycastLength))
            {
                if (Vector3.Distance(_targetPosition,  hit.point) > maxDistance)
                    _targetPosition = hit.point;
            }

            var distance = GetDistance();
            
            if(debug)
                Debug.Log(distance);

            if (_inPosition && distance > maxDistance)
            {
                _inPosition = false;
            }

            if (!_inPosition)
            {
                var lerpPosition = _vectorLinear.Update(Time.deltaTime, _targetPosition);
                var x = Mathf.PI * Mathf.InverseLerp(maxDistance - tolerance, tolerance, distance);
                var y = (float) Math.Sin(x) * yMultiplier;
                
                _target.position = lerpPosition + _target.up * y; 
                
                if (distance <= tolerance)
                    _inPosition = true;
            }
        }

        public void SetRoot(Transform root)
        {
            _root = root;
            if (_target)
            {
                _target.SetParent(_root.parent);
                _target.SetSiblingIndex(_root.GetSiblingIndex());
            }
        }
        
        public void SetTarget(Transform target)
        {
            _target = target;
        }
        
        public void SetCoefficient(float legHolderOffsetCoefficient)
        {
            var position = _attachedToBodyTarget.position;
            position += _root.forward.normalized * maxDistance * legHolderOffsetCoefficient;
            _attachedToBodyTarget.position = position;
        }

        private float GetDistance()
        {
            var plane = new Plane(_root.up, _root.position);
            var start = plane.ClosestPointOnPlane(_targetPosition);
            var end = plane.ClosestPointOnPlane(_target.position);
            return Vector3.Distance(start, end);
        }
        
        private void OnDrawGizmos()
        {
            if (!_target) return;
            
            Gizmos.color = Color.green;
            
            Gizmos.DrawWireSphere(_target.position, 0.1f);
            var startPosition = _target.position + _target.up * raycastSettings.yOffset;
            var endPosition = startPosition - _target.up * raycastSettings.raycastLength;
            Gizmos.DrawLine(startPosition, endPosition);

            if (_attachedToBodyTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_attachedToBodyTarget.position, 0.1f);
            }

            if (_targetPosition != Vector3.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_targetPosition, 0.1f);
            }
        }
    }
}