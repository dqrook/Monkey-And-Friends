using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessController
    {
        public Transform monkeyPos;

        void Update()
        {
            if(mode != ControllerMode.Dragon || gameStatus != GameStatus.Active)
            {
                return;
            }
            animator.SetBool("fly", true);
            float zMove = Time.deltaTime * forwardSpeed;
            move.z = zMove;
            move.y = 0;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));

            if(IsShifting(Direction.Left))
            {
                Shift(Direction.Left);
            }
            else if(IsShifting(Direction.Right))
            {
                Shift(Direction.Right);
            }
            if(IsAttacking())
            {
                Attack();
            }
        }
    }
}
