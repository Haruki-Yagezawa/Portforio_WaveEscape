using UnityEngine;
using UnityEngine.Events;

namespace GameSystem
{
    /// <summary>
    /// ÉMÉAèÛë‘ÇÃä«óùÉNÉâÉX
    /// </summary>
    [RequireComponent(typeof(InputDetector))]
    public class AccelerationStatus : MonoBehaviour
    {
        private bool _isParking = false;

        private bool _isStop = false;

        private EnumGearState _gearState = EnumGearState.Drive;

        [SerializeField]
        private InputDetector _inputDetector;


        public EnumGearState GearState => _gearState;

        [HideInInspector]
        public UnityEvent OnGearChange = new();


        private void Start()
        {
            SetDelegate();
        }

        private void SetDelegate()
        {
            _inputDetector.OnChange += CheckGearState;
        }

        private void CheckGearState()
        {
            if (_isStop)
            {
                _gearState = EnumGearState.Stop;
            }

            else if (_isParking)
            {
                _gearState = EnumGearState.Parking;
            }

            else if (_inputDetector.IsBrakeButton)
            {
                _gearState = EnumGearState.Brake;
            }

            else if (_inputDetector.IsAccelButton)
            {
                _gearState = EnumGearState.Drive;
            }

            else
            {
                _gearState = EnumGearState.Neutral;
            }

            OnGearChange.Invoke();
            return;
        }


        public void SetIsParking(bool isParking)
        {
            _isParking = isParking;
            CheckGearState();
        }

        public void SetIsStop(bool isStop)
        {
            _isStop = isStop;
            CheckGearState();
        }

    }
}