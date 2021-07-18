using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessRabby : EndlessScroller
    {
        public Renderer rabbyBody;
        Animator anim;
        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }

        protected override void Start() {}

        public void Initialize(Material mat = null)
        {
            if(mat != null && rabbyBody != null)
            {
                rabbyBody.material = mat;
            }
        }

        public void Reset() {}

        void OnCollisionEnter(Collision other)
        {
            Message.Send(new RunnerDie());
        }
    }
}
