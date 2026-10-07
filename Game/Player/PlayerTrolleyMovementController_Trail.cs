using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using StateBase = StateMachine.StateMachine<Player.PlayerTrolleyMovementController>.StateBase;

namespace Player
{
    /// <summary>
    /// 前に移動するクラス_トレイルに沿って移動する
    /// </summary>
    public class PlayerTrolleyMovementController_Trail : StateBase
    {
        public override int StateID => (int)PlayerTrolleyMovementController.EnumMovementType.Trail;

        internal protected override void OnInitialize()
        {
            SetDelegate();
        }

        internal protected override void OnEnter() 
        {
            SetData();
            EnterTransformPosition();
        }

        internal protected override void OnUpdate()
        {
            CheckTransformTransform();
        }

        internal protected override void OnFixedUpdate() { }

        internal protected override void OnExit() 
        {
            CheckNotHitObstacleAsync().Forget();
            ExitTransformPosition();
            ResetData();
        }


        private int _course;

        private float _progressRate;

        private SplineContainer _splineContainer;

        // ノーミスでカーブを曲がり切ったかを確認する変数
        private bool _isNotHitObstacleOnCurve = false;

        // スピードブーストをくぐった回数を保存しておく変数
        private int _speedBoostCount = 0;
        private const int SPEED_BOOST_SUCCESS_COUNT = 2;
        private const int SPEED_BOOST_SUCCESS_COUNT_SPECIAL = 3;

        // 成功時の追加スピード
        private const int ADD_SPEED_ADD_SUCCESS_CURVE_HARF = 2;
        private const int ADD_SPEED_ADD_SUCCESS_CURVE = 5;
        private const int ADD_SPEED_ADD_SUCCESS_SPECIAL = 3;


        // デリゲートを設定
        private void SetDelegate()
        {
            Owner.GetSpeedManager.OnMissMove += OnMiss;

            Owner.GetSpeedManager.OnSpeedBoost += OnSpeedBoost;
            Owner.GetGameManager.OnEndProgressEvent += CheckHitBoostBoad;
        }

        // 状態が変化して開始するときに変数を初期化する関数
        private void SetData()
        {
            _isNotHitObstacleOnCurve = true;
            _speedBoostCount = 0;

            _splineContainer = Owner.GetStageStatus.GetSplineContainer;

            Owner.StageCurveExitPointTransform = Owner.GetStageStatus.GetNextStagePoint;


            float3 splineStartPoint = _splineContainer.EvaluatePosition(0);
            float3 splineEndPoint= _splineContainer.EvaluatePosition(1);

            Vector3 splineStartPosition = new (splineStartPoint.x, splineStartPoint.y, splineStartPoint.z);
            Vector3 splineEndPosition = new (splineEndPoint.x, splineEndPoint.y, splineEndPoint.z);

            float lengthToStart = (splineStartPosition - Owner.GetTransform.position).magnitude;
            float lengthToEnd = (splineEndPosition - Owner.GetTransform.position).magnitude;

            if (lengthToStart < lengthToEnd)
            {
                _course = 1;
                _progressRate = 0;
            }
            else
            {
                _course = -1;
                _progressRate = 1;
            }
        }
        // 状態が変化して終了するときの関数
        private void ResetData()
        {
            _progressRate = 0;
        }

        // デリゲートを通じて失敗したら読み込まれる関数
        private void OnMiss()
        {
            _isNotHitObstacleOnCurve = false;
        }
        // 無傷でカーブを曲がり切ってたらスピードボーナス
        private async UniTaskVoid CheckNotHitObstacleAsync()
        {
            // 区画内でミスをしていたら終了
            if (!_isNotHitObstacleOnCurve) return;

            // ゲームが動いてない場合は終了
            if (!Owner.GetGameManager.IsMovingGame.Value) return;


            // 加算スピードを決定する
            int addSpeedAdd = 0;
            switch (Owner.GetOldStageRoom)
            {
                case GameSystem.EnumStageRoom.RightCurve:
                case GameSystem.EnumStageRoom.LeftCurve:
                    // カーブの場合
                    addSpeedAdd = ADD_SPEED_ADD_SUCCESS_CURVE;
                    break;

                case GameSystem.EnumStageRoom.RightCurveHarf:
                case GameSystem.EnumStageRoom.LeftCurveHarf:
                    // 45度カーブの場合
                    addSpeedAdd = ADD_SPEED_ADD_SUCCESS_CURVE_HARF;
                    break;

                default:
                    // カーブでなかった場合は終了
                    return;
            }


            // スピードを加速
            Owner.GetSpeedManager.AddSpeedAdd(addSpeedAdd);
            Owner.GetSpeedManager.AccelerateToMaxSpeed();

            // もしマックススピード値が高い状態での成功であれば追加演出を出す
            if (Owner.GetSpeedManager.AllMaxSpeed.Value + Owner.GetSpeedManager.SpeedAdd.Value > 160)
            {
                await Owner.GetTrailCurveEffetc.OnCurveSuccessEffectAsync();
            }

            // カーブ成功演出
            Owner.GetGameManager.SpeedUpEffect();
        }

        // ブーストしたときに読み込まれる関数
        private void OnSpeedBoost()
        {
            ++_speedBoostCount;
        }
        // プログレス（落下部屋など）が終了したときに読み込まれる関数
        // スピードアップをすべてくぐっていたらスピードボーナス
        private void CheckHitBoostBoad()
        {
            CheckHitBoostBoadAsync().Forget();
        }
        private async UniTaskVoid CheckHitBoostBoadAsync()
        {
            // ゲームが動いてない場合は終了
            if (!Owner.GetGameManager.IsMovingGame.Value) return;

            // スピード加速ボードにすべて振れられていない場合は終了
            if (SPEED_BOOST_SUCCESS_COUNT > _speedBoostCount) return;


            // スピードを加速
            Owner.GetSpeedManager.AddSpeedAdd(ADD_SPEED_ADD_SUCCESS_SPECIAL);
            Owner.GetSpeedManager.AccelerateToMaxSpeed();

            // もしス指定回数以上スピードボードに振れていた場合、追加演出を出す
            if (SPEED_BOOST_SUCCESS_COUNT_SPECIAL <= _speedBoostCount)
            {
                await Owner.GetTrailCurveEffetc.OnCurveSuccessEffectAsync();
            }

            // カーブ成功演出
            Owner.GetGameManager.SpeedUpEffect();
        }


        //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
        // 座標の初期化処理
        private void EnterTransformPosition()
        {
            NativeSpline nativeSpline = new (_splineContainer.Spline, _splineContainer.transform.localToWorldMatrix);
            SplineUtility.GetNearestPoint(nativeSpline, Owner.GetTransform.position, out float3 nearestPos, out float percent);

            _progressRate = percent;
            Owner.SetTransfromPosition(nearestPos);
        }

        // 座標変更
        private void CheckTransformTransform()
        {
            float addRange = Owner.GetSpeedManager.Speed.Value / _splineContainer.CalculateLength() * GameTime.deltaTime;

            _progressRate += _course * addRange;

            _splineContainer.Evaluate(_progressRate, out float3 spPosition, out float3 spTangent, out float3 spUpVector);



            Vector3 nextPosition = spPosition;
            Quaternion nextRotation = Quaternion.LookRotation(spTangent, spUpVector);


            Owner.SetTransfromPosition(nextPosition);

            Owner.SetTransformRotation(nextRotation);
        }

        // 部屋の退出処理
        private void ExitTransformPosition()
        {
            Owner.SetTransfromPosition(Owner.StageCurveExitPointTransform.position);

            Owner.SetTransformRotation(Owner.StageCurveExitPointTransform.rotation);
        }
    }
}