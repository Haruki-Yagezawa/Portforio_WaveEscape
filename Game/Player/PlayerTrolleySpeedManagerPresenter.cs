using GameSystem;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 移動スピード管理クラスのプレゼンター
    /// </summary>
    public class PlayerTrolleySpeedManagerPresenter : MonoBehaviour
    {
        [SerializeField]
        private PlayerTrolleySpeedManager _playerTrolleySpeedManager;

        [SerializeField]
        private GameManager _gameManager;

        [SerializeField]
        private float _stageChangeSpeed = 20;

        private void Start()
        {
            _gameManager.OnChangeStage += ChangeStage;

            float startMaxSpeed = 0;
            if (_gameManager.GetSelectGameRulesData.RulesType == GameRules.EnumGameRulesType.TimeAttack)
                startMaxSpeed = _gameManager.GetSelectGameRulesData.StartMaxSpeed;
            else if (_gameManager.GetSelectGameRulesData.RulesType == GameRules.EnumGameRulesType.Tutorial)
                startMaxSpeed = 80;

            _playerTrolleySpeedManager.SetData(startMaxSpeed);
        }

        private void ChangeStage()
        {
            _playerTrolleySpeedManager.SetMaxSpeedAdd(_stageChangeSpeed);
        }
    }
}