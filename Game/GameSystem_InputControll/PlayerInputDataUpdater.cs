using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSystem
{
    /// <summary>
    /// PlayerInputから入力を検知するクラス
    /// </summary>
    public class PlayerInputDataUpdater : MonoBehaviour
    {
        [SerializeField]
        PlayerInput _playerInput;

        [SerializeField]
        InputDetector _inputDetector;

        private void Start()
        {
            SetDelegate();
        }

        private void SetDelegate()
        {
            //_playerInput.actions["AccelPress"].performed += OnInputAccelPress;
            //_playerInput.actions["AccelRelease"].performed += OnInputAccelRelease;
            _playerInput.actions["BrakePress"].performed += OnInputBrakePress;
            _playerInput.actions["BrakeRelease"].performed += OnInputBrakeRelease;
            _playerInput.actions["JumpPress"].performed += OnInputJumpPress;
            _playerInput.actions["JumpRelease"].performed += OnInputJumpRelease;
            _playerInput.actions["Rudder"].performed += OnInputRudder;
            _playerInput.actions["Rudder"].canceled += OnInputRudder;
            _playerInput.actions["RightSideBoostPress"].performed += OnInputRightSideBoostPress;
            _playerInput.actions["LeftSideBoostPress"].performed += OnInputLeftSideBoostPress;

            _playerInput.actions["AnyKeyPress"].performed += OnInputAnyKeyPress;
            _playerInput.actions["AnyKeyRelease"].performed += OnInputAnyKeyRelease;

            _playerInput.actions["Test_UIButtonClearKeyPress"].performed += OnInputTest_UIButtonClearKeyPress;
        }

        private void OnInputAccelPress(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsAccelButton(true);
        }
        private void OnInputAccelRelease(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsAccelButton(false);
        }

        private void OnInputBrakePress(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsBrakeButton(true);
        }
        private void OnInputBrakeRelease(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsBrakeButton(false);
        }

        private void OnInputJumpPress(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsJumpButton(true);
        }
        private void OnInputJumpRelease(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsJumpButton(false);
        }

        private void OnInputRudder(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsRudderValue(context.ReadValue<float>());
        }

        public void OnInputRightSideBoostPress(InputAction.CallbackContext context)
        {
            _inputDetector.SetOnRightSideBoost();
        }

        public void OnInputLeftSideBoostPress(InputAction.CallbackContext context)
        {
            _inputDetector.SetOnLeftSideBoost();
        }

        public void OnInputAnyKeyPress(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsAnyKey(true);
        }
        public void OnInputAnyKeyRelease(InputAction.CallbackContext context)
        {
            _inputDetector.SetIsAnyKey(false);
        }


        //========================================================================
        // テスト用
        private bool _isUIButtonClear = false;
        public void OnInputTest_UIButtonClearKeyPress(InputAction.CallbackContext context)
        {
            _isUIButtonClear = !_isUIButtonClear;
            _inputDetector.OnInputTest_UIButtonClear.Value = _isUIButtonClear;
        }
    }
}