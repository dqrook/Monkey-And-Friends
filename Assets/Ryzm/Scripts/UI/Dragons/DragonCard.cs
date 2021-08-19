using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using Ryzm.UI.Messages;
using TMPro;
using Ryzm.Dragon;

namespace Ryzm.UI
{
    public class DragonCard : MonoBehaviour
    {
        #region Public Variables
        public TextMeshProUGUI priceText;
        public RectTransform rectTransform;
        public RawImage dragonImage;
        public Image image;
        public CanvasGroup group;
        public Texture texture;
        #endregion

        #region Private Variables
        int dragonIndex = -1;
        DragonCardMetadata[] dragonCards;
        #endregion

        #region Event Functions
        void Awake()
        {
            if(group == null)
            {
                group = GetComponent<CanvasGroup>();
            }
            dragonCards = new DragonCardMetadata[0];
            Message.AddListener<DragonCardMetadataResponse>(OnDragonCardMetadataResponse);
        }

        void OnEnable()
        {
            SetGroupActive(false);
            Message.Send(new DragonCardMetadataRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonCardMetadataResponse>(OnDragonCardMetadataResponse);
        }
        #endregion

        #region Listener Functions
        void OnDragonCardMetadataResponse(DragonCardMetadataResponse response)
        {
            dragonCards = response.dragonCards;
            UpdateUI();
        }
        #endregion

        #region Public Functions
        public void UpdateCard(int dragonIndex, float newWidth, float newHeight)
        {
            this.dragonIndex = dragonIndex;
            UpdateUI();
            UpdateDimensions(newWidth, newHeight);
        }

        public void UpdateDimensions(float newWidth, float newHeight)
        {
            if(rectTransform.sizeDelta.x != newWidth || rectTransform.sizeDelta.y != newHeight)
            {
                rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            }
        }

        public void OnClickView()
        {
            Debug.Log("clicked");
        }

        public void Deactivate()
        {
            dragonIndex = -1;
            gameObject.SetActive(false);
        }
        #endregion

        #region Private Functions
        void UpdateUI()
        {
            if(dragonIndex >= 0 && dragonCards != null && dragonIndex < dragonCards.Length)
            {
                DragonCardMetadata card = dragonCards[dragonIndex];
                texture = card.image;
                dragonImage.texture = card.image;
                priceText.text = card.price + " Near";
                if(group != null && group.alpha != 1)
                {
                    group.alpha = 1;
                }
                SetGroupActive(true);
            }
            else
            {
                SetGroupActive(false);
            }
        }

        void SetGroupActive(bool isActive)
        {
            if(group != null)
            {
                if(isActive && group.alpha != 1)
                {
                    group.alpha = 1;
                }
                else if(!isActive && group.alpha != 0)
                {
                    group.alpha = 0;
                }
            }
        }
        #endregion
    }
}
