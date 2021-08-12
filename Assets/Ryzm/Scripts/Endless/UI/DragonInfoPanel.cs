using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class DragonInfoPanel : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public List<DragonGenePanel> panels = new List<DragonGenePanel>();
        public ColumnResizer resizer;
        #endregion

        #region Private Variables
        DragonGenes genes;
        #endregion
        
        #region Event Functions
        void Awake()
        {
            if(canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            Message.AddListener<DragonGenesResponse>(OnDragonGenesResponse);
            Message.AddListener<EnableDragonInfoPanel>(OnEnableDragonInfoPanel);
            Message.AddListener<DisableDragonInfoPanel>(OnDisableDragonInfoPanel);
        }

        void Start()
        {
            Message.Send(new DragonGenesRequest("dragonInfoPanel"));
        }
        
        void OnDestroy()
        {
            Message.RemoveListener<DragonGenesResponse>(OnDragonGenesResponse);
            Message.RemoveListener<EnableDragonInfoPanel>(OnEnableDragonInfoPanel);
            Message.RemoveListener<DisableDragonInfoPanel>(OnDisableDragonInfoPanel);
        }
        #endregion

        #region Listener Functions
        void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(genes == null && response.receiver == "dragonInfoPanel")
            {
                genes = response.genes;
            }
        }

        void OnEnableDragonInfoPanel(EnableDragonInfoPanel enable)
        {
            Enable(enable.singleDragonData);
        }

        void OnDisableDragonInfoPanel(DisableDragonInfoPanel disable)
        {
            Disable();
        }
        #endregion

        #region Private Functions
        void Enable(DragonResponse data)
        {
            foreach(DragonGenePanel panel in panels)
            {
                switch(panel.type)
                {
                    case GeneType.Body:
                        DragonGene gene = genes.GetGeneBySequence(data.bodyGenes.ToArray());
                        gene.rawSequence = data.bodyGenesSequence;
                        panel.Initialize(gene);
                        break;
                    case GeneType.Wing:
                        DragonGene wingGene = genes.GetGeneBySequence(data.wingGenes.ToArray());
                        wingGene.rawSequence = data.wingGenesSequence;
                        panel.Initialize(wingGene);
                        break;
                    case GeneType.Horn:
                        DragonGene hornGene = genes.GetGeneBySequence(data.hornGenes.ToArray(), data.hornTypeGenes.ToArray());
                        hornGene.rawSequence = data.hornGenesSequence;
                        panel.Initialize(hornGene);
                        break;
                    case GeneType.Moves:
                        DragonGene movesGene = genes.GetGeneBySequence(data.moveGenes.ToArray(), true);
                        movesGene.rawSequence = data.moveGenesSequence;
                        panel.Initialize(movesGene);
                        break;
                    default:
                        break;
                }
            }
            canvas.enabled = true;
        }

        void Disable()
        {
            foreach(DragonGenePanel panel in panels)
            {
                panel.Disable();
            }
            canvas.enabled = false;
            resizer.Disable();
        }
        #endregion
    }

}
