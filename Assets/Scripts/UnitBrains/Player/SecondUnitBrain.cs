using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)

        // Проверка на перегрев
            if (GetTemperature() >= OverheatTemperature)
            {
                return; // Если перегрев, выходим из метода
            }

            // Генерация снарядов
            int projectileCount = GetProjectileCount();
            for (int i = 0; i < projectileCount; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            // Увеличиваем температуру после выстрела
            IncreaseTemperature();

            // Проверка на перегрев после увеличения температуры
            if (GetTemperature() >= OverheatTemperature)
            {
                _overheated = true;
                _cooldownTime = OverheatCooldown; // Устанавливаем время для охлаждения
            }
        }
// Получить количество снарядов в зависимости от температуры
private int GetProjectileCount()
        {
            if (_temperature < 1f) return 1; // Если температура меньше 1, возвращаем 1 снаряд
            if (_temperature < 2f) return 2; // Если температура меньше 2, возвращаем 2 снаряда
            return 3; // Если температура 3 или больше, возвращаем 3 снаряда
        }

        // Получить текущую температуру
        private float GetTemperature()
        {
            return _temperature; // Возвращаем текущую температуру
        }

        // Увеличить температуру на 1
        private void IncreaseTemperature()
        {
            _temperature += 1f; // Увеличиваем температуру на 1 за каждый выстрел
        }

        // Метод для охлаждения
        private void CoolDown()
        {
            if (_overheated)
            {
                _cooldownTime -= Time.deltaTime; // Уменьшаем время охлаждения
                if (_cooldownTime <= 0)
                {
                    _overheated = false; // Сбрасываем состояние перегрева
                    _temperature = 0f; // Сбрасываем температуру
                }
            }
        }

        // Метод обновления, который вызывается каждый кадр
        private void Update()
        {
            CoolDown(); // Вызываем метод охлаждения в каждом кадре
        }
            //float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            //var projectile = CreateProjectile(forTarget);
            //AddProjectileToList(projectile, intoList);
            ///////////////////////////////////////

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = GetReachableTargets();
            while (result.Count > 1)
            {
                result.RemoveAt(result.Count - 1);
            }
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}
