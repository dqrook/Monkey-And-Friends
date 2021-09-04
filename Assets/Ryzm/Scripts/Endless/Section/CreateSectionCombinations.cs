using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ryzm.EndlessRunner
{
    public class CreateSectionCombinations : MonoBehaviour
    {
        public BaseSectionPrefab baseSectionPrefab;

        public void Create()
        {
            BaseSectionPrefab prefab = Instantiate(baseSectionPrefab);
            prefab.transform.position = transform.position;
            prefab.transform.rotation = transform.rotation;


        }

        public void GetCombos()
        {
            // BaseSectionPrefab prefab = Instantiate(baseSectionPrefab);
            // prefab.transform.position = transform.position;
            // prefab.transform.rotation = transform.rotation;

            baseSectionPrefab.GetCombinations();
        }

    }
}
