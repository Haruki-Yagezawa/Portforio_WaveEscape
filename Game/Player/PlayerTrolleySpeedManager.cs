using UnityEngine;
using GameSystem;
using Status.Player;
using Status.Equipment;
using R3;
using System;

namespace Player
{
    /// <summary>
    /// 移動スピード管理クラス
    /// </summary>
    public class PlayerTrolleySpeedManager : MonoBehaviour
    {
        [SerializeField]
        private AccelerationStatus _accelerationStatus;

        [SerializeField]
        private PlayerStatusEquipmentData _equipmentData;

        [SerializeField]
        private SpeedData _so_SpeedData;

        private PlayerTrolleySpeedManager_SpeedBoost _speedBoost = new();

        private BroomSpeedData _speedData;

        private float _irregularAccel = 0;
        private float _irregularAccel_Flame = 0;

        //private const float DEC_SPEED = 32;


        public ReactiveProperty<float> Speed = new(0);

        public ReactiveProperty<float> SpeedAdd = new(0);

        public ReactiveProperty<float> SpeedBoostAdd = new(0);

        public ReactiveProperty<float> AllMaxSpeed = new(0);

        public float MaxSpeed => _speedData.MaxSpeed;

        [HideInInspector]
        public event Action OnMissMove;

        [HideInInspector]
        public event Action OnSpeedBoost;


        public void SetData(float maxSpeed = 0)
        {
            _speedData = _equipmentData.BroomData.SpeedData;

            float setMaxSpeed = maxSpeed == 0 ? _speedData.MaxSpeed : maxSpeed;
            float setSpeedAdd = _speedData.SpeedAdd;

            AllMaxSpeed.Value = setMaxSpeed;
            SpeedAdd.Value = setSpeedAdd;

            _so_SpeedData.maxSpeed = setMaxSpeed;
            Speed.Value = setMaxSpeed + setSpeedAdd;
        }

        private void Update()
        {
            CheckSpeedValue();
        }

        /// <summary>
        /// 速度を計算する
        /// </summary>
        private void CheckSpeedValue()
        {
            float nextSpeed = Speed.Value;

            //===============================================
            // スピード加算
            switch (_accelerationStatus.GearState)
            {
                case EnumGearState.Parking:
                    nextSpeed = 0;
                    break;
                case EnumGearState.Stop:
                    break;
                default:
                    nextSpeed += CheckAccelerationValue();
                    break;
            }

            // 反映
            float maxSpeed = AllMaxSpeed.Value + SpeedAdd.Value;
            _so_SpeedData.maxSpeed = maxSpeed;

            // 修正
            if (nextSpeed < 0) nextSpeed = 0;
            if (nextSpeed > maxSpeed) nextSpeed = maxSpeed;

            //===============================================
            // ブースト分を加算する
            _speedBoost.OnUpdate();
            SpeedBoostAdd.Value = _speedBoost.OnBoostCheck();
            nextSpeed += SpeedBoostAdd.Value;

            //===============================================
            Speed.Value = nextSpeed;
        }

        /// <summary>
        /// 加速度を計算する
        /// </summary>
        /// <returns></returns>
        private float CheckAccelerationValue()
        {
            float accelerationValue = 0;

            switch (_accelerationStatus.GearState)
            {
                case EnumGearState.Drive:
                    accelerationValue += _speedData.Drive;
                    break;
                case EnumGearState.Brake:
                    accelerationValue += _speedData.Brake;
                    break;
                case EnumGearState.Neutral:
                    accelerationValue += _speedData.Neutral;
                    break;
            }

            accelerationValue += _irregularAccel;

            accelerationValue += _irregularAccel_Flame;
            _irregularAccel_Flame = 0;

            return accelerationValue * GameTime.deltaTime;
        }

        /// <summary>
        /// ミスをしたときにスピードを減算する
        /// </summary>
        /// <param name="subSpeed"></param>
        /// <param name="missRatio"></param>
        public void OnMiss(int subSpeed, float missRatio)
        {
            if (SpeedAdd.Value > 0)
            {
                SpeedAdd.Value -= subSpeed;
                SpeedAdd.Value = Mathf.Max(0, SpeedAdd.Value);
            }
            else
            {
                Speed.Value -= AllMaxSpeed.Value * missRatio;
            }

            OnMissMove?.Invoke();
        }

        /// <summary>
        /// スピードを最大にする
        /// </summary>
        public void AccelerateToMaxSpeed()
        {
            Speed.Value = AllMaxSpeed.Value + SpeedAdd.Value;
        }

        /// <summary>
        /// 加算スピードに加算
        /// </summary>
        /// <param name="value"></param>
        public void AddSpeedAdd(in float value)
        {
            SpeedAdd.Value += value;
        }

        /// <summary>
        /// 加算最速スピードに加算
        /// </summary>
        /// <param name="value"></param>
        public void SetMaxSpeedAdd(in float value)
        {
            AllMaxSpeed.Value += value;
        }

        //======================================================
        /// <summary>
        /// 追い風や水抵抗などの特殊な加算スピードを変更
        /// </summary>
        /// <param name="value"></param>
        public void SetIrregularAccel(in float value)
        {
            _irregularAccel = value;
        }

        /// <summary>
        /// フレームごとにイベントを発行してしまう処理用　特殊な加算スピード
        /// </summary>
        /// <param name="value"></param>
        public void SetIrregularAccel_Collider(float value)
        {
            _irregularAccel_Flame += value;
        }

        //======================================================
        /// <summary>
        /// スピードブーストをスタートさせる
        /// </summary>
        public void OnStartSpeedBoost()
        {
            _speedBoost.OnBoostStart();

            // スピードブースト！！
            OnSpeedBoost?.Invoke();
        }
    }
}