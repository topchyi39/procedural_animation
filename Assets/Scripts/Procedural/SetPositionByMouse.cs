using System;
using UnityEngine;

namespace Procedural
{
    public class SetPositionByMouse : MonoBehaviour
    {
        [SerializeField] private ProceduralMoving _proceduralMoving;

        private bool _clicked;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            _clicked = Input.GetMouseButton(0);
        }

        private void FixedUpdate()
        {
            if (_clicked)
            {
                var ray = GetRay();
                Debug.DrawRay(ray.origin, ray.direction * 100f);
                if (Physics.Raycast(ray, out var hit, 100f))
                {
                    _proceduralMoving.MoveToPosition(hit.point);
                }
            }
        }

        private Ray GetRay()
        {
            return _camera.ScreenPointToRay(Input.mousePosition);
        }
    }
}