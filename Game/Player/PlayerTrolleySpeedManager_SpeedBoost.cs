using UnityEngine;

namespace Player
{
    /// <summary>
    /// 移動スピード管理クラスのスピードブースト処理用クラス
    /// </summary>
    public class PlayerTrolleySpeedManager_SpeedBoost
    {
        private bool _isBoost = false;
        private float _time = 0;
        private float _speed = 0;

        private const float BOOST_TIME = 1;
        private const float BOOST_SPEED = 20;

        /// <summary>
        /// ブーストをスタートさせる
        /// </summary>
        public void OnBoostStart()
        {
            _speed = BOOST_SPEED;
            _time = BOOST_TIME;
            _isBoost = true;
        }

        public void OnUpdate()
        {
            if (!_isBoost) return;

            // 時間が0だった場合はスピードを減算
            if (_time == 0)
            {
                _speed -= GameTime.deltaTime * 80;

                // スピードが0以下になっていた場合はブースト終了
                if (_speed < 0)
                {
                    _isBoost = false;
                    _speed = 0;
                }

                return;
            }

            // まだ時間が立ってない場合は時間を減算
            _time -= GameTime.deltaTime;

            // 時間が0以下になっていた場合はブーストを減算し始める
            if (_time < 0)
            {
                _time = 0;
            }
        }

        public float OnBoostCheck()
        {
            if (_isBoost) return _speed;
            else return 0;
        }
    }
}