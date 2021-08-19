using UnityEngine;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class ScrollHider : MonoBehaviour
    {
        static public float contentTop;
        static public float contentBottom;

        static public bool HideObject(CanvasGroup canvasGroup, float givenPosition, float givenHeight)
        {
            if ((Mathf.Abs(givenPosition) + givenHeight > contentTop && Mathf.Abs(givenPosition) + givenHeight < contentBottom) || (Mathf.Abs(givenPosition) > contentTop && Mathf.Abs(givenPosition) < contentBottom))
            {
                if (canvasGroup.alpha != 1)
                {
                    canvasGroup.alpha = 1;
                }
                return true;
            }
            else
            {
                if (canvasGroup.alpha != 0)
                {
                    canvasGroup.alpha = 0;
                }
                return false;
            }
        }

        static public void Setup(ScrollRect givenScroll)
        {
            contentTop = (1 - givenScroll.verticalNormalizedPosition) * (givenScroll.content.rect.height - givenScroll.viewport.rect.height);
            contentBottom = contentTop + givenScroll.viewport.rect.height;
        }
    }
}
