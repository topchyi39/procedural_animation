using System;
using Procedural;
using UnityEngine;

namespace SecondOrderLinear
{
    public class MovingToTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SecondOrderParameters lerpPositionParameters;
        
        private VectorLinear _vectorLinear;
        private Transform _transform;

        private void OnValidate()
        {
            _vectorLinear = new VectorLinear(lerpPositionParameters, (_transform ? _transform : transform).position);
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _vectorLinear = new VectorLinear(lerpPositionParameters, _transform.position);
        }

        private void FixedUpdate()
        {
            _transform.position = _vectorLinear.Update(Time.deltaTime, target.position);
        }
    }
}