using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando"; // Название юнита, которым управляет этот мозг.
        private const float OverheatTemperature = 3f; // Температура, при которой юнит перегревается.
        private const float OverheatCooldown = 2f; // Время, необходимое для охлаждения юнита после перегрева.
        private const float TemperatureIncreasePerShot = 1f;  // Насколько увеличивается температура за каждый выстрел.
        private const float PassiveCoolingRate = 0.5f;        // Скорость пассивного охлаждения (температура снижается в секунду).
        private float _temperature = 0f; // Текущая температура юнита.
        private float _cooldownTime = 0f; // Оставшееся время охлаждения после перегрева.
        private bool _overheated = false; // Флаг, указывающий, перегрет ли юнит.

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            CoolDown(Time.deltaTime);  // Сначала охлаждаем юнит, чтобы он успел остыть перед стрельбой.

            if (IsOverheated) // Если юнит перегрет, не стреляем.
            {
                return;
            }

            int projectileCount = GetProjectileCount(); // Определяем количество снарядов для выстрела.
            for (int i = 0; i < projectileCount; i++) // Создаем и добавляем снаряды в список.
            {
                var projectile = CreateProjectile(forTarget); // Создаем снаряд, направленный на цель.
                AddProjectileToList(projectile, intoList); // Добавляем снаряд в список для запуска.
                IncreaseTemperature(); // Увеличиваем температуру юнита после выстрела.
            }
        }

        private int GetProjectileCount()
        {
            // Определяем количество снарядов для выстрела в зависимости от текущей температуры.
            if (_temperature < 1f) return 1; // Если температура меньше 1, возвращаем 1 снаряд.
            if (_temperature < 2f) return 2; // Если температура меньше 2, возвращаем 2 снаряда.
            return 3; // Иначе возвращаем 3 снаряда.
        }

        private void IncreaseTemperature()
        {
            // Увеличиваем температуру юнита на величину TemperatureIncreasePerShot.
            _temperature += TemperatureIncreasePerShot;

            // Проверяем, не перегрелся ли юнит.
            if (_temperature >= OverheatTemperature)
            {
                _overheated = true; // Устанавливаем флаг перегрева.
                _cooldownTime = OverheatCooldown; // Запускаем процесс охлаждения.
            }
        }

        // Метод для охлаждения юнита (с пассивным охлаждением)
        private void CoolDown(float deltaTime)
        {
            if (_overheated)
            {
                // Если юнит перегрет, уменьшаем время охлаждения на deltaTime.
                _cooldownTime -= deltaTime;

                // Вычисляем значение t для линейной интерполяции температуры.
                float t = 1f - Mathf.Clamp01(_cooldownTime / OverheatCooldown);

                // Интерполируем температуру от максимальной (OverheatTemperature) до 0.
                _temperature = Mathf.Lerp(OverheatTemperature, 0f, t);

                // Если время охлаждения истекло...
                if (_cooldownTime <= 0)
                {
                    _overheated = false; // Сбрасываем флаг перегрева.
                    _cooldownTime = 0f; // Сбрасываем время охлаждения.
                    _temperature = 0f; // Устанавливаем температуру в 0.
                }
            }
            else
            {
                // Если юнит не перегрет, применяем пассивное охлаждение.
                _temperature = Mathf.Max(0f, _temperature - PassiveCoolingRate * deltaTime); // Уменьшаем температуру, но не ниже 0.
            }
        }


        // Update method that is called every frame (to handle cooling):
        public override void Update(float deltaTime, float time)
        {
            CoolDown(deltaTime); // Вызываем метод охлаждения каждый кадр.
        }

        public bool IsOverheated => _overheated; // Свойство, позволяющее узнать, перегрет ли юнит (только для чтения).

        protected override List<Vector2Int> FilterTargets (List<Vector2Int> targets)
        {
            List<Vector2Int> result = base.FilterTargets(targets); // Получаем базовый список целей, отфильтрованных родительским классом.

            if (result.Count == 0) // Если список целей пуст, возвращаем пустой список.
            {
                return result;
            }

            Vector2Int closestTarget = result[0]; // Считаем первую цель ближайшей.
            float minDistance = DistanceToOwnBase(closestTarget); // Вычисляем расстояние до первой цели.

            for (int i = 1; i < result.Count; i++) // Перебираем остальные цели.
            {
                float distance = DistanceToOwnBase(result[i]); // Вычисляем расстояние до текущей цели.
                if (distance < minDistance) // Если расстояние до текущей цели меньше минимального...
                {
                    minDistance = distance; // Обновляем минимальное расстояние.
                    closestTarget = result[i]; // Делаем текущую цель ближайшей.
                }
            }

            result.Clear(); // Очищаем список целей.
            result.Add(closestTarget); // Добавляем в список только ближайшую цель.

            return result; // Возвращаем список, содержащий только ближайшую цель.
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep(); // Используем базовый алгоритм для определения следующего шага.
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets(); // Получаем список доступных целей.
            while (result.Count > 1) // Если целей больше одной...
            {
                result.RemoveAt(result.Count - 1); // Удаляем последнюю цель из списка, оставляя только одну.
            }
            return result; // Возвращаем список с одной целью (или пустой список, если целей не было).
        }
    }
}
