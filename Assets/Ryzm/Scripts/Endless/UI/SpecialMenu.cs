using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using UnityEngine.UI;
using TMPro;

namespace Ryzm.UI
{
    public class SpecialMenu : RyzmMenu
    {
        #region Public Variables
        public Slider specialSlider;
        public GameObject useSpecialBadge;
        public TextMeshProUGUI specialText;
        #endregion

        #region Private Variables
        IEnumerator updateSpecialSlider;
        int maxSpecial;
        float currentSpecial = -1;
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
                        Message.AddListener<RunnerSpecialResponse>(OnRunnerSpecialResponse);
                        Message.AddListener<SpecialAttackResponse>(OnSpecialAttackResponse);
                        Message.Send(new RunnerSpecialRequest());
                    }
                    else
                    {
                        Message.RemoveListener<RunnerSpecialResponse>(OnRunnerSpecialResponse);
                        Message.RemoveListener<SpecialAttackResponse>(OnSpecialAttackResponse);
                        currentSpecial = -1;
                        StopAllCoroutines();
                        updateSpecialSlider = null;
                        SetSpecialBadge(false);
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
        void OnRunnerSpecialResponse(RunnerSpecialResponse response)
        {
            maxSpecial = response.maxSpecial;
            if(response.special != currentSpecial)
            {
                UpdateSpecial(response.special);
            }
        }

        void OnSpecialAttackResponse(SpecialAttackResponse response)
        {
            if(response.state == EndlessRunner.AttackState.On)
            {
                SetSpecialBadge(false);
            }
        }
        #endregion

        #region Private Functions
        void UpdateSpecial(float _special)
        {
            if(maxSpecial > 0 && _special >= 0)
            {
                currentSpecial = _special;
                if(updateSpecialSlider != null)
                {
                    StopCoroutine(updateSpecialSlider);
                    updateSpecialSlider = null;
                }
                updateSpecialSlider = UpdateSpecialSlider();
                StartCoroutine(updateSpecialSlider);
                if(currentSpecial >= maxSpecial)
                {
                    SetSpecialBadge(true);
                }
            }
        }

        void SetSpecialBadge(bool enable)
        {
            useSpecialBadge.SetActive(enable);
        }
        #endregion

        #region Coroutines
        IEnumerator UpdateSpecialSlider()
        {
            float t = 0;
            float fracSpecial = (float)currentSpecial / maxSpecial;
            float diff = Mathf.Abs(specialSlider.value - fracSpecial);
            while(diff > 0.01f)
            {
                specialSlider.value = Mathf.Lerp(specialSlider.value, fracSpecial, 5 * Time.deltaTime);
                diff = Mathf.Abs(specialSlider.value - fracSpecial);
                t += Time.deltaTime;
                yield return null;
            }
            specialSlider.value = fracSpecial;
        }
        #endregion
    }
}