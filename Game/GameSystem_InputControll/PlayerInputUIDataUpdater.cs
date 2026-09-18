using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// UIから入力を検知するクラス
    /// </summary>
    public class PlayerInputUIDataUpdater : MonoBehaviour
    {
        [SerializeField]
        InputDetector _inputDetector;

        private bool _isLeftButtonDown = false;
        private bool _isRightButtonDown = false;


        private void Start()
        {
            _inputDetector.SetIsAccelButton(true);
            _inputDetector.SetIsBrakeButton(false);
        }

        public void OnJumpButton(bool isActive)
        {
            _inputDetector.SetIsJumpButton(isActive);
        }

        public void OnMoveLeftButton(bool isActive)
        {
            _isLeftButtonDown = isActive;
            CheckMove();
        }

        public void OnMoveRightButton(bool isActive)
        {
            _isRightButtonDown = isActive;
            CheckMove();
        }

        public void OnBrakeButton(bool isActive)
        {
            _inputDetector.SetIsAccelButton(!isActive);
            _inputDetector.SetIsBrakeButton(isActive);
        }


        // 横移動ボタンから移動方向を確認する
        private void CheckMove()
        {
            // 横移動
            int moveDir = 0;
            if (_isLeftButtonDown) moveDir -= 1;
            if (_isRightButtonDown) moveDir += 1;

            _inputDetector.SetIsRudderValue(moveDir);
        }


        // 画面内の何処かが押された場合を取得する関数
        public void OnAnyKeyButton(bool isActive)
        {
            _inputDetector.SetIsAnyKey(isActive);
        }
    }
}