using R3;
using System;
using UnityEngine;


namespace GameSystem
{
    /// <summary>
    /// 入力ハブを担うクラス
    /// </summary>
    public class InputDetector : MonoBehaviour
    {
        [HideInInspector]
        public event Action OnChange;

        // 加速度要素
        private bool _isAccelButton = false;
        private bool _isBrakeButton = false;
        private bool _isJumpButton = false;

        [HideInInspector] public event Action<bool> OnAccelButton;
        [HideInInspector] public event Action<bool> OnBrakeButton;
        [HideInInspector] public event Action<bool> OnJumpButton;
        [HideInInspector] public event Action<bool> OnRudderLeft;
        [HideInInspector] public event Action<bool> OnRudderRight;
        [HideInInspector] public event Action OnRightSideBoost;
        [HideInInspector] public event Action OnLeftSideBoost;

        [HideInInspector] public event Action<bool> OnAnyKeyButton;
        [HideInInspector] public ReactiveProperty<bool> OnInputTest_UIButtonClear = new();


        public bool IsAccelButton => _isAccelButton;
        public bool IsBrakeButton => _isBrakeButton;
        public bool IsJunpButton => _isJumpButton;
        public ReactiveProperty<float> RudderValue = new();

        private void Start()
        {
            RudderValue
            .Subscribe(value =>
            {
                if (value == 0)
                {
                    OnRudderRight?.Invoke(false);
                    OnRudderLeft?.Invoke(false);
                    return;
                }

                if (value > 0) OnRudderRight?.Invoke(true);
                else OnRudderLeft?.Invoke(true);
            })
            .AddTo(this);
        }

        public void SetIsAccelButton(bool isAccel)
        {
            _isAccelButton = isAccel;
            OnChange?.Invoke();
            OnAccelButton?.Invoke(isAccel);
        }

        public void SetIsBrakeButton(bool isBrake)
        {
            _isBrakeButton = isBrake;
            OnChange?.Invoke();
            OnBrakeButton?.Invoke(isBrake);
        }

        public void SetIsJumpButton(bool isJump)
        {
            _isJumpButton = isJump;
            OnChange?.Invoke();
            OnJumpButton?.Invoke(isJump);
        }

        public void SetIsRudderValue(float isRudder)
        {
            RudderValue.Value = isRudder;
            OnChange?.Invoke();
        }


        public void SetIsAnyKey(bool isPush)
        {
            OnAnyKeyButton?.Invoke(isPush);
        }


        //=============================================
        public void SetOnRightSideBoost()
        {
            OnRightSideBoost?.Invoke();
        }
        public void SetOnLeftSideBoost()
        {
            OnLeftSideBoost?.Invoke();
        }
    }
}