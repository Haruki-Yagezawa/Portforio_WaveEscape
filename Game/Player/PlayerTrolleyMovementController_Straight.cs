using UnityEngine;
using StateBase = StateMachine.StateMachine<Player.PlayerTrolleyMovementController>.StateBase;

namespace Player
{
    /// <summary>
    /// 前に移動するクラス_目の前を直線に移動する
    /// </summary>
    public class PlayerTrolleyMovementController_Straight : StateBase
    {
        public override int StateID => (int)PlayerTrolleyMovementController.EnumMovementType.Straight;

        internal protected override void OnInitialize() { }

        internal protected override void OnEnter() { }

        internal protected override void OnUpdate()
        {
            CheckTransformPosition();
        }

        internal protected override void OnFixedUpdate() { }

        internal protected override void OnExit() { }


        private void CheckTransformPosition()
        {
            Vector3 vector = new (0, 0, Owner.GetSpeedManager.Speed.Value * GameTime.deltaTime);

            Vector3 nextPosition = Owner.GetTransform.position + (Owner.GetTransform.rotation * vector);

            Owner.SetTransfromPosition(nextPosition);
        }
    }
}