using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class WorldItemsResponse : Message
    {
        public Camera mainCamera;
        public Light mainLight;
        public int gameClipPlane;
        public int gameFieldOfView;

        public WorldItemsResponse(Camera mainCamera, Light mainLight, int gameClipPlane, int gameFieldOfView)
        {
            this.mainCamera = mainCamera;
            this.mainLight = mainLight;
            this.gameClipPlane = gameClipPlane;
            this.gameFieldOfView = gameFieldOfView;
        }
    }
}
