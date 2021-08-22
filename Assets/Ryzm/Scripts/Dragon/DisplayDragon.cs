using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeControl;
using Ryzm.Dragon.Messages;

namespace Ryzm.Dragon
{
    public class DisplayDragon : MarketDragon
    {
        #region Public Variables
        public int dragonIndex;
        public Canvas canvas;
        public TextMeshProUGUI nearPriceText;
        public TextMeshProUGUI dollarsPriceText;
        #endregion

        #region Private Variables
        string price;
        MarketDragonMetadata data;
        bool isEnabled;
        Quaternion camRotation;
        Transform camTrans;
        bool initializedCamera;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            camTrans = Camera.main.transform;
        }
        #endregion

        #region Public Functions
        public void Enable(MarketDragonMetadata data)
        {
            if(!initializedCamera)
            {
                camRotation = camTrans.rotation;
                initializedCamera = true;
            }
            isEnabled = true;
            UpdateData(data);
            EnableMaterials();
        }

        public void Disable()
        {
            isEnabled = false;
            DisableMaterials();
        }

        public void ReEnable()
        {
            if(isEnabled)
            {
                Enable(this.data);
            }
        }

        public void DisableCanvas()
        {
            canvas.enabled = false;
        }

        public override void DisableMaterials()
        {
            base.DisableMaterials();
            DisableCanvas();
        }

        public override void EnableMaterials()
        {
            base.EnableMaterials();
            canvas.transform.LookAt(canvas.transform.position + camRotation * Vector3.forward, camRotation * Vector3.up);
            canvas.enabled = true;
        }

        public void OnClickCanvas()
        {
            Debug.Log("OnClick");
            Message.Send(new DisplayDragonZoomRequest(dragonIndex, data.id));
        }
        #endregion

        #region Private Functions
        void UpdateData(MarketDragonMetadata data)
        {
            this.data = data;
            hornType = data.hornType;
            price = data.price;
            bodyPath = "Dragon/" + data.bodyGenes + "/" + data.primaryColor;
            wingPath = "Dragon/" + data.wingGenes + "0/" + data.primaryColor;
            hornPath = "Dragon/" + data.hornGenes + "0/" + data.secondaryColor;
            UpdateDragons();
            activeDragon.Fly(true);
            UpdateCanvas();
        }

        void UpdateCanvas()
        {
            if(!canvas.enabled)
            {
                canvas.enabled = true;
            }
            nearPriceText.text = price + " Near";
        }
        #endregion
    }
}
