using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class TestSection : MonoBehaviour
    {
        void Start()
        {
            Message.Send(new ResumeGame());
        }
    }
}
