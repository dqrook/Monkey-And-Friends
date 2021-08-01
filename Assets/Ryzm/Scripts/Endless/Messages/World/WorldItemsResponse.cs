using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class WorldItemsResponse : Message
    {
        public Camera mainCamera;
        public Light mainLight;
        public int gameFieldOfView;

        public WorldItemsResponse(Camera mainCamera, Light mainLight, int gameFieldOfView)
        {
            this.mainCamera = mainCamera;
            this.mainLight = mainLight;
            this.gameFieldOfView = gameFieldOfView;
        }
    }
}
