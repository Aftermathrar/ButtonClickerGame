﻿using System.Collections;
using System.Collections.Generic;
using ButtonGame.Inventories;
using ButtonGame.Saving;
using ButtonGame.Stats;
using ButtonGame.Stats.Enums;
using ButtonGame.Stats.Follower;
using GameDevTV.Utils;
using UnityEngine;

namespace ButtonGame.Attributes
{
    public class Fullness : MonoBehaviour, ISaveable, IAttribute
    {
        LazyValue<float> fullnessPoints;
        
        float maxCapacity;
        BaseStats baseStats;
        BodyManager bodyManager;

        private void Awake() 
        {
            fullnessPoints = new LazyValue<float>(GetInitialFullness);
            maxCapacity = fullnessPoints.value;
            bodyManager = GetComponent<BodyManager>();
        }

        private float GetInitialFullness()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Capacity);
        }

        private void Start() 
        {
            fullnessPoints.ForceInit();
        }

        public void RecalculateMaxCapacity()
        {
            maxCapacity = GetInitialFullness();
        }

        public float DigestFood(float foodAmount)
        {
            if(fullnessPoints.value == 0) return 0;

            foodAmount = Mathf.Min(fullnessPoints.value, foodAmount);
            float foodPercent = foodAmount / fullnessPoints.value;
            fullnessPoints.value = Mathf.Max(0, fullnessPoints.value - foodAmount);
            float cal = bodyManager.Digest(foodPercent);
            float foodRatio = cal / foodAmount;
            return foodRatio;
        }

        public void EatFood(ConsumableItem food, int number)
        {
            float cal = food.GetCalories() * number;
            bodyManager.AddCalories(cal);

            float size = food.GetSize() * number;
            fullnessPoints.value += size;
        }

        public void GainAttribute(float amount)
        {
            fullnessPoints.value += amount;
        }

        public float GetAttributeValue()
        {
            return fullnessPoints.value;
        }

        public float GetMaxAttributeValue()
        {
            return maxCapacity;
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return fullnessPoints.value / maxCapacity;
        }

        public object CaptureState()
        {
            return fullnessPoints.value;
        }

        public void RestoreState(object state)
        {
            fullnessPoints.value = (float)state;
        }
    }
}
