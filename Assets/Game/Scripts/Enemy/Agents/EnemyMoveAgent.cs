using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class EnemyMoveAgent : MonoBehaviour
    {
        public bool IsReached => _isReached;

        private Vector2 _destination;
        private bool _isReached;

        public event Action<bool> OnReached;
        public event Action<Vector2> OnMove;


        public void SetDestination(Vector2 endPoint)
        {
            _destination = endPoint;
            _isReached = false;
        }

        public void TryMove(float fixedDeltaTime)
        {
            if (_isReached) return;

            var vector = _destination - (Vector2)transform.position;
            if (vector.magnitude <= 0.25f)
            {
                _isReached = true;
                OnReached?.Invoke(_isReached);
                return;
            }

            Vector2 direction = vector.normalized * fixedDeltaTime;
            OnMove?.Invoke(direction);
        }
    }
}