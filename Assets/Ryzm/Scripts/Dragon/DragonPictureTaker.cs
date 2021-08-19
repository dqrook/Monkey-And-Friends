using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class DragonPictureTaker : MonoBehaviour
    {
        #region Public Variables
        public int targetHornType = 1;
        public string resolution = "512";
        public DragonGenes genes;
        public DragonPicturePaths picturePaths;
        public List<HornDragonMap> dragonMap = new List<HornDragonMap>();
        #endregion

        #region Private Variables
        List<string> geneSequences = new List<string>();
        List<string> colors = new List<string>
        {
            "0", "1", "2", "3", "4", "5", "6", "7"
        };

        WaitForSeconds wait4Frames;
        #endregion

        #region Event Functions
        void Awake()
        {
            // AddDragonPicturePaths();
            wait4Frames = new WaitForSeconds(4 * Time.deltaTime);
        }

        void Start()
        {
            TakePictures();
        }
        #endregion

        #region Private Functions
        void AddDragonPicturePaths()
        {
            foreach(DragonGene gene in genes.dragonGenes)
            {
                geneSequences.Add(gene.sequence);
            }
            foreach(string color2 in colors)
            {
                foreach(string color1 in colors)
                {
                    foreach(string hornSequence in geneSequences)
                    {
                        foreach(string wingSequence in geneSequences)
                        {
                            foreach(string bodySequence in geneSequences)
                            {
                                DragonPicturePath path = new DragonPicturePath();
                                path.bodyPath = bodySequence + "/" + color1;
                                path.wingPath = wingSequence + "/" + color1;
                                path.hornPath = hornSequence + "/" + color2;
                                path.desktopPath = bodySequence + "/" + wingSequence + "/" + hornSequence + "/" + color1 + "/" + color2 + "/" + resolution + ".png";
                                picturePaths.paths.Add(path);
                            }
                        }
                    }
                }
            }
        }

        void TakePictures()
        {
            int index = targetHornType - 1;
            if(targetHornType < 0 || targetHornType > 4)
            {
                Debug.LogError("CHECK YOUR HORN TYPE CHIEF");
            }
            else
            {
                foreach(HornDragonMap map in dragonMap)
                {
                    map.dragon.gameObject.SetActive(targetHornType == map.hornType);
                }
                StartCoroutine(_TakePictures(dragonMap[index]));
            }
        }

        IEnumerator _TakePictures(HornDragonMap dragon)
        {
            string hornType = dragon.hornType.ToString();
            DragonResponse res = new DragonResponse();
            int numPictures = picturePaths.paths.Count;
            int dex = 0;
            while(dex < numPictures)
            {
                DragonPicturePath path = picturePaths.paths[dex];
                string desktopPath = "/Users/ryzm/Desktop/Dragons/" + hornType + "/" + path.desktopPath;
                res.bodyTexture = path.bodyPath;
                res.wingTexture = path.wingPath;
                res.hornTexture = path.hornPath;
                res.backTexture = path.hornPath;
                dragon.dragon.data = res;
                dragon.dragon.GetTextures();
                yield return wait4Frames;
                UnityEngine.ScreenCapture.CaptureScreenshot(desktopPath, 1);
                yield return wait4Frames;
                dex++;
                Debug.Log(dex.ToString() + "/" + numPictures);
                yield return null;
            }
            yield break;
        }
        #endregion
    }

    [System.Serializable]
    public struct HornDragonMap
    {
        public int hornType;
        public BaseDragon dragon;
    }
}
