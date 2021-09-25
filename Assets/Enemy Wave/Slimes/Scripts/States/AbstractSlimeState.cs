using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public abstract class AbstractSlimeState : FSM.State
	{
		//public Animator slimeAnimator;
		public Transform slimeTransform;
		protected SlimeCore slimeCore;
		protected SlimeController slimeController;
		public SlimePhysicsUtils slimePhysicsUtils;


		#region ANIMATOR PARAMETERS
		protected const string JUMP_BOOL = "ON_JUMPING";
		protected const string IDLE_TRIGGER = "IDLE";
		protected const string IDLE_BOOL = "ON_IDLE";
		protected const string ABDUCTING_BOOL = "ON_ABDUCTING";
		protected const string ABSORPTION_COMPLETED_BOOL = "ON_ABSORPTION_COMPLETED";
		protected const string ABSORPTION_COMPLETED_TRIGGER = "ON_ABSORPTION_COMPLETED";
		protected const string EAT_CANDY_TRIGGER = "EAT_CANDY";
		#endregion

		public enum State
		{
			JUMPING = 0,
			IDLE = 1,
			FALLING_DOWN = 6,
			DISABLED = 7,
		}

		public State state;

		public virtual void Init(SlimeController slimeController, SlimeCore slimeCore)
		{
			this.slimeController = slimeController;
			this.slimeCore = slimeCore;
		}
	}
}