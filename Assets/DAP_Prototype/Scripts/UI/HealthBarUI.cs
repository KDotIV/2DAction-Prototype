using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public Slider slider;

        public void SetMaxHealth(float health)
        {
            slider.maxValue = health;
            slider.value = health;
        }
        public void SetHealth(float health)
        {
            slider.value = health;
        }

        public HealthBarUI GetHealthBarUI()
        {
            return this;
        }
    }
}
