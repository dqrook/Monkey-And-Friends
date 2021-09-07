using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class HealthMenu : RyzmMenu
    {
        #region Public Variables
        public Slider healthSlider;
        #endregion

        #region Private Variables
        int currentHealth;
        int maxHealth;
        IEnumerator updateHealthSlider;
        #endregion

        #region Properties
        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        Message.AddListener<RunnerHealthResponse>(OnRunnerHealthResponse);
                        Message.Send(new RunnerHealthRequest());
                    }
                    else
                    {
                        Message.RemoveListener<RunnerHealthResponse>(OnRunnerHealthResponse);
                        currentHealth = -1;
                        StopAllCoroutines();
                        updateHealthSlider = null;
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
        void OnRunnerHealthResponse(RunnerHealthResponse response)
        {
            maxHealth = response.maxHealth;
            if(response.health != currentHealth)
            {
                UpdateHealth(response.health);
            }
        }
        #endregion

        #region Private Functions
        void UpdateHealth(int _health)
        {
            if(maxHealth > 0 && _health >= 0)
            {
                currentHealth = _health;
                // Debug.Log(currentHealth);
                if(updateHealthSlider != null)
                {
                    StopCoroutine(updateHealthSlider);
                    updateHealthSlider = null;
                }
                updateHealthSlider = UpdateHealthSlider(currentHealth);
                StartCoroutine(updateHealthSlider);
            }
        }
        #endregion

        #region Coroutines
        IEnumerator UpdateHealthSlider(int _health)
        {
            float fracHealth = (float)currentHealth / maxHealth;
            // Debug.Log(currentHealth + " " + maxHealth + " " + _health);
            float diff = Mathf.Abs(healthSlider.value - fracHealth);
            while(diff > 0.01f)
            {
                // Debug.Log(fracHealth + " " + currentHealth + " " + maxHealth + " " + _health);
                // Debug.Log(fracHealth + " " + healthSlider.value);
                healthSlider.value = Mathf.Lerp(healthSlider.value, fracHealth, 5 * Time.deltaTime);
                diff = Mathf.Abs(healthSlider.value - fracHealth);
                yield return null;
            }
            healthSlider.value = fracHealth;
        }
        #endregion
    }
}

