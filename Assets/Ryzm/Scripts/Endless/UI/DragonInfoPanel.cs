using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;
using Ryzm.Blockchain;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.UI.Messages;
using TMPro;

namespace Ryzm.UI
{
    public class DragonInfoPanel : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public ColumnResizer resizer;

        [Header("Genes")]
        public List<DragonGenePanel> genePanels = new List<DragonGenePanel>();

        [Header("Market")]
        public List<DragonPricePanel> pricePanels = new List<DragonPricePanel>();

        [Header("Edit Sale Popup")]
        public Canvas editSalePopup;
        public TMP_InputField priceInput;
        public TextMeshProUGUI updateButtonText;
        public GameObject removeFromMarketButton;

        [Header("Buy Popup")]
        public Canvas buyPopup;

        [Header("Updating Popup")]
        public TextMeshProUGUI updateText;
        public Canvas updatingPanel;

        [Header("Success Popup")]
        public Canvas successPanel;
        public TextMeshProUGUI successText;

        [Header("Error Popup")]
        public Canvas errorPanel;
        public TextMeshProUGUI errorText;
        

        #endregion

        #region Private Variables
        DragonGenes genes;
        string accountName;
        DragonResponse currentDragon;
        bool addingNewDragon;
        bool isActive;
        #endregion
        
        #region Event Functions
        void Awake()
        {
            if(canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            Message.AddListener<LoginResponse>(OnLoginResponse);
            Message.AddListener<DragonGenesResponse>(OnDragonGenesResponse);
            Message.AddListener<EnableDragonInfoPanel>(OnEnableDragonInfoPanel);
            Message.AddListener<DisableDragonInfoPanel>(OnDisableDragonInfoPanel);
            Message.AddListener<AddDragonToMarketResponse>(OnAddDragonToMarketResponse);
            Message.AddListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);
        }

        void Start()
        {
            Message.Send(new DragonGenesRequest("dragonInfoPanel"));
        }
        
        void OnDestroy()
        {
            Message.RemoveListener<LoginResponse>(OnLoginResponse);
            Message.RemoveListener<DragonGenesResponse>(OnDragonGenesResponse);
            Message.RemoveListener<EnableDragonInfoPanel>(OnEnableDragonInfoPanel);
            Message.RemoveListener<DisableDragonInfoPanel>(OnDisableDragonInfoPanel);
            Message.RemoveListener<AddDragonToMarketResponse>(OnAddDragonToMarketResponse);
            Message.RemoveListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);
        }
        #endregion

        #region Properties
        bool IsUser
        {
            get
            {
                if(currentDragon == null)
                {
                    return false;
                }
                return accountName == currentDragon.owner;
            }
        }
        #endregion

        #region Listener Functions
        void OnLoginResponse(LoginResponse response)
        {
            if(response.status == LoginStatus.LoggedIn)
            {
                accountName = response.accountName;
            }
        }
        void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(genes == null && response.receiver == "dragonInfoPanel")
            {
                genes = response.genes;
            }
        }

        void OnEnableDragonInfoPanel(EnableDragonInfoPanel enable)
        {
            if(!isActive)
            {
                isActive = true;
                Enable(enable.singleDragonData);
            }
        }

        void OnDisableDragonInfoPanel(DisableDragonInfoPanel disable)
        {
            if(isActive)
            {
                isActive = false;
                Disable();
            }
        }

        void OnAddDragonToMarketResponse(AddDragonToMarketResponse response)
        {
            if(isActive)
            {
                if(response.status == TransactionStatus.Success)
                {
                    successText.text = addingNewDragon ? "Successfully added dragon to market for " + response.data.price + " Near" : "Successfully updated price to " + priceInput.text + " Near";
                    successPanel.enabled = true;
                    errorPanel.enabled = false;
                    updatingPanel.enabled = false;
                    Enable(response.data);
                }
                else if(response.status == TransactionStatus.Failed)
                {
                    errorText.text = addingNewDragon ? "Unable to add dragon to market" : "Unable to update the dragon's price";
                    successPanel.enabled = false;
                    errorPanel.enabled = true;
                    updatingPanel.enabled = false;
                }
            }
        }

        void OnRemoveDragonFromMarketResponse(RemoveDragonFromMarketResponse response)
        {
            if(response.status == TransactionStatus.Success)
            {
                Enable(response.data);
                successText.text = "Successfully removed dragon from market!";
                successPanel.enabled = true;
                errorPanel.enabled = false;
                updatingPanel.enabled = false;
            }
            else if(response.status == TransactionStatus.Failed)
            {
                errorText.text = "Unable to remove dragon from market. Please try again";
                successPanel.enabled = false;
                errorPanel.enabled = true;
                updatingPanel.enabled = false;
            }
        }
        #endregion

        #region Public Functions
        public void OpenMarketPopup(bool isForSale)
        {
            if(IsUser)
            {
                priceInput.text = currentDragon.price.ToString();
                updateButtonText.text = currentDragon.price > 0 ? "Update Price" : "Add To Market";
                removeFromMarketButton.SetActive(currentDragon.price > 0);
                editSalePopup.enabled = true;
            }
            else
            {
                buyPopup.enabled = true;
            }
        }

        public void CloseMarketPopup()
        {
            editSalePopup.enabled = false;
            updatingPanel.enabled = false;
            buyPopup.enabled = false;
        }

        public void UpdatePrice()
        {
            string priceText = priceInput.text;
            float price = -1;
            bool isNumber = float.TryParse(priceText, out price);
            if(isNumber)
            {
                Debug.Log("new price is " + price);
                if(price <= 0)
                {
                    updateText.text = "Removing dragon from market";
                    updatingPanel.enabled = true;
                    editSalePopup.enabled = false;
                }
                else
                {
                    int dragonId = currentDragon.id;
                    Message.Send(new AddDragonToMarketRequest(dragonId, price));
                    addingNewDragon = !(currentDragon.price > 0);
                    updateText.text = addingNewDragon ? "Adding dragon to market for " + price + " Near" : "Updating price to " + price + " Near";
                    updatingPanel.enabled = true;
                    editSalePopup.enabled = false;
                }
            }
            else
            {
                Debug.LogError("Gotta b a number chief");
            }
        }

        public void RemoveFromMarket()
        {
            if(currentDragon.price > 0)
            {
                int dragonId = currentDragon.id;
                Message.Send(new RemoveDragonFromMarketRequest(dragonId));
                updateText.text = "Removing dragon from market";
                updatingPanel.enabled = true;
                editSalePopup.enabled = false;
            }
            else
            {
                Debug.LogError("that button shouldnt be showing chief");
            }
        }

        public void CancelTransaction()
        {
            CloseMarketPopup();
        }

        public void CloseStatusPanels()
        {
            updatingPanel.enabled = false;
            successPanel.enabled = false;
            errorPanel.enabled = false;
        }
        #endregion

        #region Private Functions
        void Enable(DragonResponse data)
        {
            currentDragon = data;
            resizer.Enable();
            foreach(DragonGenePanel panel in genePanels)
            {
                switch(panel.type)
                {
                    case GeneType.Body:
                        DragonGene gene = genes.GetGeneBySequence(data.bodyGenes.ToArray(), GeneType.Body);
                        gene.rawSequence = data.bodyGenesSequence;
                        panel.Initialize(gene);
                        break;
                    case GeneType.Wing:
                        DragonGene wingGene = genes.GetGeneBySequence(data.wingGenes.ToArray(), GeneType.Wing);
                        wingGene.rawSequence = data.wingGenesSequence;
                        panel.Initialize(wingGene);
                        break;
                    case GeneType.Horn:
                        DragonGene hornGene = genes.GetGeneBySequence(data.hornGenes.ToArray(), data.hornTypeGenes.ToArray());
                        hornGene.rawSequence = data.hornGenesSequence;
                        panel.Initialize(hornGene);
                        break;
                    case GeneType.Moves:
                        DragonGene movesGene = genes.GetGeneBySequence(data.moveGenes.ToArray(), GeneType.Moves);
                        movesGene.rawSequence = data.moveGenesSequence;
                        panel.Initialize(movesGene);
                        break;
                    default:
                        break;
                }
            }
            foreach(DragonPricePanel panel in pricePanels)
            {
                panel.Initialize(data, IsUser);
            }
            canvas.enabled = true;
        }

        void Disable()
        {
            foreach(DragonGenePanel panel in genePanels)
            {
                panel.Disable();
            }
            editSalePopup.enabled = false;
            canvas.enabled = false;
            currentDragon = null;
            resizer.Disable();
        }
        #endregion
    }

}
