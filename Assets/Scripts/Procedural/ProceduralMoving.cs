using System;
using SecondOrderLinear;
using UnityEngine;

namespace Procedural
{
    [Serializable]
    public class SecondOrderParameters
    {
        [SerializeField] private float f = 1f;
        [SerializeField] private float z = 1f;
        [SerializeField] private float r = 0f;

        public float F => f;
        public float Z => z;
        public float R => r;
    }
    
    public class ProceduralMoving : MonoBehaviour
    {
        [Serializable]
        private class LegHolder
        {
            public Leg leg;
            [Range(0f, 1f)] public float offsetCoefficient;
        }

        [SerializeField] private Transform target;
        [SerializeField] private float positionTolerance = 0.01f;
        [SerializeField] private float rotationTolerance = 0.5f;
        [SerializeField] private SecondOrderParameters positionParameters;
        [SerializeField] private SecondOrderParameters rotationParameters;
        
        [SerializeField] private LegHolder[] legsHolder;
        [SerializeField] private Body body;
        
        private Vector3 _targetPosition;
        private Vector3 _targetDirection;
        
        private VectorLinear _positionLinear;
        private VectorLinear _directionLinear;
        private float _startDistance;
        private bool _startMoving;
        
        /// <summary>
        /// Validate fields, find legs into childs
        /// </summary>
        private void Validate()
        {
            body = GetComponentInChildren<Body>();
            var array = GetComponentsInChildren<Leg>();
            
            legsHolder = new LegHolder[array.Length];

            var index = 0;
            for (var i = 0; i < array.Length; i++)
            {
                var leg = array[index++];
                var offsetCoefficient = i % 2 == 0 ? 0f : 0.2f;
                legsHolder[i] = new LegHolder
                {
                    leg = leg,
                    offsetCoefficient = offsetCoefficient
                };
            }
        }

        private void Start()
        {
            foreach (var legHolder in legsHolder)
            {
                legHolder.leg.SetRoot(transform);
                legHolder.leg.SetCoefficient(legHolder.offsetCoefficient);
            }
            
            _positionLinear = new VectorLinear(positionParameters, transform.position);
            _directionLinear = new VectorLinear(rotationParameters, transform.forward);
        }

        private void FixedUpdate()
        {
            var position = transform.position;
            
            if(_targetPosition == position) return;
            
            
            
            UpdateRotation(position);
            UpdatePosition(position);
        }

        private void UpdateRotation(Vector3 previousPosition)
        {
            var currentDistanceToTarget = Vector3.Distance(_targetPosition, previousPosition);
            if (currentDistanceToTarget <= rotationTolerance) return;

            var forwardDirection = Vector3.Lerp(transform.forward, _targetDirection,
                Mathf.InverseLerp(_startDistance, rotationTolerance, currentDistanceToTarget));
            
            if (forwardDirection != Vector3.zero)
                transform.forward = forwardDirection;
        }

        private void UpdatePosition(Vector3 previousPosition)
        {
            if (Vector3.Distance(_targetPosition, previousPosition) <= positionTolerance) return;
            
            var lerpPosition = _positionLinear.Update(Time.deltaTime, _targetPosition);
            transform.position = lerpPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, .001f, transform.localScale.z));
        }

        public void MoveToPosition(Vector3 hitInfoPoint)
        {
            _targetPosition = hitInfoPoint;
            var position = transform.position;
            _targetDirection = (_targetPosition - position).normalized;
            _startDistance = Vector3.Distance(_targetPosition, position);
        }
    }
}