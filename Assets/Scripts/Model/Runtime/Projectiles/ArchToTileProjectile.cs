using UnityEngine;

namespace Model.Runtime.Projectiles
{
    public class ArchToTileProjectile : BaseProjectile
    {
        private const float ProjectileSpeed = 7f;
        private readonly Vector2Int _target;
        private readonly float _timeToTarget;
        private readonly float _totalDistance;
        
        public ArchToTileProjectile(Unit unit, Vector2Int target, int damage, Vector2Int startPoint) : base(damage, startPoint)
        {
            _target = target;
            _totalDistance = Vector2.Distance(StartPoint, _target);
            _timeToTarget = _totalDistance / ProjectileSpeed;
        }

        protected override void UpdateImpl(float deltaTime, float time)
        {
            float timeSinceStart = time - StartTime;
            float t = timeSinceStart / _timeToTarget;
            
            Pos = Vector2.Lerp(StartPoint, _target, t);
            
            float localHeight = 0f;
            float totalDistance = _totalDistance;

            ///////////////////////////////////////
            // Insert you code here
            ///////////////////////////////////////
            float timeOffset = t - 0.5f; // Calculating `t-0.5`
            float timeOffsetSquared = Mathf.Pow(timeOffset, 2); // Calculating (t-0.5)^2
            float parabolicBase = -4f * timeOffsetSquared + 1f; // Calculating -4 * (t-0.5)^2 + 1

            float maxHeight = 0.6f * totalDistance; // Calculates the max height as 60% of totalDistance

            localHeight = parabolicBase * maxHeight; // Calculate the final localHeight
            ///////////////////////////////////////
            // End of the code to insert
            ///////////////////////////////////////
            
            Height = localHeight;
            if (time > StartTime + _timeToTarget)
                Hit(_target);
        }
    }
}
