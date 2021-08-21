using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class FilterDragon : MarketDragon
    {
        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            UpdateFilters(new MarketFilter[0]);
        }
        #endregion

        #region Public Functions
        public void UpdateFilters(MarketFilter[] filters)
        {
            string primaryColor = "";
            string secondaryColor = "";
            string bodyGenes = "";
            string wingGenes = "";
            string hornGenes = "";
            hornType = "1";
            foreach(MarketFilter filter in filters)
            {
                switch(filter.type)
                {
                    case FilterType.PrimaryColor:
                        primaryColor = filter.value;
                        break;
                    case FilterType.SecondaryColor:
                        secondaryColor = filter.value;
                        break;
                    case FilterType.BodyGenes:
                        bodyGenes = filter.value;
                        break;
                    case FilterType.WingGenes:
                        wingGenes = filter.value;
                        break;
                    case FilterType.HornGenes:
                        hornGenes = filter.value;
                        break;
                    case FilterType.HornType:
                        hornType = filter.value;
                        break;
                }
            }

            bodyPath = "Dragon/Plain/default";
            wingPath = "Dragon/Plain/default";
            hornPath = "Dragon/Plain/default";
            if(primaryColor.Length > 0)
            {
                if(bodyGenes.Length > 0)
                {
                    bodyPath = "Dragon/" + bodyGenes + "/" + primaryColor;
                }
                else
                {
                    bodyPath = "Dragon/Plain/" + primaryColor;
                }
                if(wingGenes.Length > 0)
                {
                    wingPath = "Dragon/" + wingGenes + "/" + primaryColor;
                }
                else
                {
                    wingPath = "Dragon/Plain/" + primaryColor;
                }
            }
            else if(bodyGenes.Length > 0 || wingGenes.Length > 0)
            {
                if(bodyGenes.Length > 0)
                {
                    bodyPath = "Dragon/" + bodyGenes + "/0";
                }
                if(wingGenes.Length > 0)
                {
                    wingPath = "Dragon/" + wingGenes + "/0";
                }
            }

            if(secondaryColor.Length > 0)
            {
                if(hornGenes.Length > 0)
                {
                    hornPath = "Dragon/" + hornGenes + "/" + secondaryColor;
                }
                else
                {
                    hornPath = "Dragon/Plain/" + secondaryColor;
                }
            }
            else if(hornGenes.Length > 0)
            {
                hornPath = "Dragon/" + hornGenes + "/0";
            }

            UpdateDragons();
            activeDragon.Fly(true);
        }

        public void Reset()
        {
            bodyPath = "Dragon/Plain/default";
            wingPath = "Dragon/Plain/default";
            hornPath = "Dragon/Plain/default";
            hornType = "1";
            UpdateDragons();
        }
        #endregion
    }
}
