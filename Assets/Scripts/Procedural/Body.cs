using UnityEngine;

namespace Procedural
{
    public class Body : MonoBehaviour
    {
        private Vector3 _targetDirection;
        
        public void UpdateDirection(Vector3 targetDirection)
        {
            _targetDirection = targetDirection;
        }
    }
}