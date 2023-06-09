using System;
using SecondOrderLinear;
using UnityEngine;

namespace DefaultNamespace
{
    public class Move : MonoBehaviour
    {
        [SerializeField] private float f;
        [SerializeField] private float z;
        [SerializeField] private float r;
        [SerializeField] private Transform target;

        private VectorLinear _linear;
        private void OnValidate()
        {
            Init();
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _linear = new VectorLinear(f, z, r, transform.position);
        }

        private void FixedUpdate()
        {
            transform.position = _linear.Update(Time.deltaTime, target.position);
        }

    }
}