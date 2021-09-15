using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using UnityEngine.UI;
using TMPro;

namespace Ryzm.UI
{
    public class HealthMenu : RyzmMenu
    {
        #region Public Variables
        public Slider healthSlider;
        public GameObject damagedBadge;
        public TextMeshProUGUI damagedText;
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
                        SetDamagedBadge(false);
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
                int damage = currentHealth - _health;
                currentHealth = _health;
                // Debug.Log(currentHealth);
                if(updateHealthSlider != null)
                {
                    StopCoroutine(updateHealthSlider);
                    updateHealthSlider = null;
                }
                updateHealthSlider = UpdateHealthSlider(damage);
                StartCoroutine(updateHealthSlider);
            }
        }

        void SetDamagedBadge(bool enable, int damage = 0)
        {
            damagedBadge.SetActive(enable);
            if(enable)
            {
                damagedText.text = "-" + damage.ToString();
            }
        }
        #endregion

        #region Coroutines
        IEnumerator UpdateHealthSlider(int damage)
        {
            if(damage > 0)
            {
                SetDamagedBadge(true, damage);
            }
            else
            {
                SetDamagedBadge(false);
            }
            float t = 0;
            float fracHealth = (float)currentHealth / maxHealth;
            // Debug.Log(currentHealth + " " + maxHealth + " " + _health);
            float diff = Mathf.Abs(healthSlider.value - fracHealth);
            while(diff > 0.01f)
            {
                // Debug.Log(fracHealth + " " + currentHealth + " " + maxHealth + " " + _health);
                // Debug.Log(fracHealth + " " + healthSlider.value);
                healthSlider.value = Mathf.Lerp(healthSlider.value, fracHealth, 5 * Time.deltaTime);
                diff = Mathf.Abs(healthSlider.value - fracHealth);
                t += Time.deltaTime;
                yield return null;
            }
            healthSlider.value = fracHealth;
            if(damage > 0)
            {
                float damageTime = 2;
                while(t < damageTime)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            SetDamagedBadge(false);
        }
        #endregion
    }
}

