using Cysharp.Threading.Tasks;
using GameSystem;
using Stage;
using Stage.Fall;
using StateMachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 前に移動するクラス
    /// </summary>
    public class PlayerTrolleyMovementController : MonoBehaviour, IWarpable
    {
        private StateMachine<PlayerTrolleyMovementController> _stateMachine;

        private StageRoomElementRoadStatus _stageStatus;

        private EnumStageRoom _oldStageRoom;

        [SerializeField]
        private GameManager _gameManager;

        [SerializeField]
        private AccelerationStatus _accelerationStatus;

        [SerializeField]
        private PlayerTrolleySpeedManager _playerTrolleySpeedManager;

        [SerializeField]
        private PlayerTrolleyMovementController_Trail_CurveEffect _trailCurveEffect;


        [HideInInspector]
        public Transform StageCurveExitPointTransform;

        public GameManager GetGameManager => _gameManager;

        public StageRoomElementRoadStatus GetStageStatus => _stageStatus;

        public EnumStageRoom GetOldStageRoom => _oldStageRoom;

        public EnumStageRoom SetStageRoom(EnumStageRoom stageRoom) => _oldStageRoom = stageRoom;

        public PlayerTrolleySpeedManager GetSpeedManager => _playerTrolleySpeedManager;

        public PlayerTrolleyMovementController_Trail_CurveEffect GetTrailCurveEffetc => _trailCurveEffect;

        public Transform GetTransform => transform;


        private void Start()
        {
            // ステートマシンのインスタンスを作成
            _stateMachine = new StateMachine<PlayerTrolleyMovementController>(this);

            // どの状態からでもIdleStateへ遷移できるように設定
            _stateMachine.AddAnyTransition<PlayerTrolleyMovementController_Straight>((int)EnumMovementType.Straight);
            _stateMachine.AddAnyTransition<PlayerTrolleyMovementController_Trail>((int)(EnumMovementType.Trail));

            // ステートマシンを開始し、初期状態をIdleStateに設定
            _stateMachine.StartStateMachine<PlayerTrolleyMovementController_Straight>((int)EnumMovementType.Straight);
        }

        private void Update()
        {
            if (_accelerationStatus.GearState == EnumGearState.Parking ||
                _accelerationStatus.GearState == EnumGearState.Stop) return;

            // ステートマシン更新
            _stateMachine.UpdateStateMahine();
        }

        /// <summary>
        /// ステージルームのコライダーにあたった場合の処理
        /// </summary>
        /// <param name="stageStatus"></param>
        public async UniTaskVoid OnTriggerStageRoomAsync(StageRoomElementStatusBase stageStatus)
        {
            StageRoomElementRoadStatus roadStatus = stageStatus.GetComponent<StageRoomElementRoadStatus>();

            switch (stageStatus.GetStageRoom)
            {
                case EnumStageRoom.Straight:
                    _stageStatus = roadStatus;
                    _stateMachine.ChangeState((int)EnumMovementType.Straight);
                    break;
                case EnumStageRoom.RightCurve:
                case EnumStageRoom.LeftCurve:
                case EnumStageRoom.RightCurveHarf:
                case EnumStageRoom.LeftCurveHarf:
                case EnumStageRoom.TrailStraight:
                case EnumStageRoom.Fall:
                    _stageStatus = roadStatus;
                    _stateMachine.ChangeState((int)EnumMovementType.Trail);
                    break;
            }

            // Fallだった場合
            if(stageStatus.GetStageRoom == EnumStageRoom.Fall)
            {
                // Fallのオブジェクトに配置された加速ボードの数を抜き取る
                StageElementFallManager fallManager = stageStatus.GetComponent<StageElementFallManager>();

                // 特殊距離の処理を開始することをGameManagerWithLayoutBaseに通知する
                _gameManager.StartProgress(roadStatus.GetSplineContainer.CalculateLength() - 
                                           roadStatus.SplineContainer_OutLength);
            }

            await UniTask.Yield();
            _oldStageRoom = stageStatus.GetStageRoom;
        }

        /// <summary>
        /// 座標を変える処理
        /// </summary>
        /// <param name="position"></param>
        public void SetTransfromPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetTransformRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        /// <summary>
        /// イベントIDの定義
        /// </summary>
        public enum EnumMovementType : int
        {
            Straight = 0,
            Trail,
        }


        //-------------------------------------------------------------------------------
        // IWarpable
        public void OnWarp(Transform point)
        {
            transform.SetPositionAndRotation(point.position, point.rotation);
        }
    }
}