using System;
using SecondOrderLinear;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procedural
{
    public class ProceduralMoving : MonoBehaviour
    {
        [Serializable]
        private class LegHolder
        {
            public Leg leg;
            [Range(0f, 1f)] public float offsetCoefficient;
        }

        [SerializeField] private Transform target;
        
        [SerializeField] private float f = 1f;
        [SerializeField] private float z = 1f;
        [SerializeField] private float r = 0f;
        
        [SerializeField] private LegHolder[] legsHolder;

        private Vector3 _previousPosition;
        private Vector3 _targetPosition;
        private VectorLinear positionLinear;
        private VectorLinear rotationLinear;
        
        private void Validate()
        {
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
            
            positionLinear = new VectorLinear(f, z, r, transform.position);
            rotationLinear = new VectorLinear(f, z, r, transform.forward);

        }

        private void FixedUpdate()
        {
            if(_targetPosition == _previousPosition) return;
            
            var lerpPosition = positionLinear.Update(Time.deltaTime, _targetPosition);
            var forwardDirection = rotationLinear.Update(Time.deltaTime,(target.position - _previousPosition).normalized);
            if(forwardDirection != Vector3.zero)
                transform.forward = forwardDirection;
            transform.position = lerpPosition;
                
                _previousPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, .001f, transform.localScale.z));
        }

        public void MoveToPosition(Vector3 hitInfoPoint)
        {
            _targetPosition = hitInfoPoint;
        }
    }
}