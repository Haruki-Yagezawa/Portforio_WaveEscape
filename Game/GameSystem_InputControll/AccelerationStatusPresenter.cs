using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// ギア状態の管理クラスのプレゼンター
    /// </summary>
    public class AccelerationStatusPresenter : MonoBehaviour
    {
        [SerializeField]
        private AccelerationStatus _accelerationStatus;

        [SerializeField]
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager.OnChangeStage += ChangeStage;
        }


        private void ChangeStage()
        {
            _accelerationStatus.SetIsStop(false);
        }
    }
}