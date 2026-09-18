using Cysharp.Threading.Tasks;
using GameRules;
using GameSystem;
using PlayerAccountSystems;
using SoundContainer;
using UnityEngine;

namespace Ranking
{
    /// <summary>
    /// ランキングUIの全体を管理するクラス
    /// </summary>
    public class UIRankingManager : MonoBehaviour
    {
        [SerializeField]
        private UIRankingViewController _viewController;

        [SerializeField]
        private GameManagerWithLayoutBase _layoutBase;

        [SerializeField]
        private GameObject _objectRoot;

        [Header("音関連")]
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private SoundDataScriptableObject _soundData;


        private bool _isRunningOpen = false;
        private bool _isRunningClose = false;


        private void Start()
        {
            _viewController.OnStart();
        }

        //-----------------------------------------------------------------
        /// <summary>
        /// UIを表示する
        /// </summary>
        /// <param name="gameRules"></param>
        public async void OnPushRankingOpenAsync(GameRulesDataScriptableObject gameRules)
        {
            if (_isRunningOpen) return;
            _isRunningOpen = true;

            try
            {
                // ログイン確認処理
                if (!await PlayerAccountManager.CheckLoginAsync())
                {
                    // 失敗した場合は終了
                    return;
                }


                // ＝＝＝＝＝ ロードUIを表示 ＝＝＝＝＝
                await PlayerAccountManager.OnActiveLoadUI(true);

                try
                {
                    // プレイヤーの最大スコアを取得する
                    (bool isSuccess, float score) getScore;
                    getScore = await PlayFabRankingManager.OnGetPlayerScore(gameRules.RankingDataName, gameRules.RankingType);

                    // UIに表示する
                    if (getScore.isSuccess)
                    {
                        // プレイヤーのスコアを設定する
                        _viewController.OnSetUserRankingData(getScore.score);
                    }

                    // ランキングを設定する
                    (bool isSuccess, float[] scores, string[] names) rankingData = await PlayFabRankingManager.OnGetRankingAsync(gameRules.RankingDataName, gameRules.RankingType);
                    if (!rankingData.isSuccess) return;
                    await _viewController.OnSetRankingElementsDataAsync(gameRules.RankingType, rankingData.scores, rankingData.names);

                    // 音を再生
                    _soundData.PlayOneShotSound(_audioSource, EnumSoundName.SE_System_OpenUI);

                    //// UIを表示する
                    _objectRoot.SetActive(true);
                    _viewController.OnShowUI(0.2f);
                }
                finally
                {
                    // ＝＝＝＝＝ ロードUIを非表示 ＝＝＝＝＝
                    await PlayerAccountManager.OnActiveLoadUI(false);

                }
            }
            finally
            {
                // 開く処理の完了（フラグ解除）
                _isRunningOpen = false;
            }
        }

        //-----------------------------------------------------------------
        /// <summary>
        /// UIを非表示にする
        /// </summary>
        public async void OnPushRankingCloseAsync()
        {
            if (_isRunningClose) return;
            _isRunningClose = true;

            try
            {
                // 音を再生
                _soundData.PlayOneShotSound(_audioSource, EnumSoundName.SE_System_CloseUI);

                // UIを非表示にする
                await _viewController.OnHideUIAsync(0.3f);

                _objectRoot.SetActive(false);
            }
            finally
            {
                _isRunningClose = false;
            }
        }

        //-----------------------------------------------------------------
        /// <summary>
        /// スコア登録
        /// </summary>
        /// <param name="gameRules"></param>
        /// <returns></returns>
        public async UniTask OnRegisterScoreInRankingAsync(GameRulesDataScriptableObject gameRules)
        {
            // スコア
            float userRankingScoreValue = 0;

            // スコアを取得する
            switch (gameRules.RankingType)
            {
                case EnumRankingType.Time:
                    userRankingScoreValue = _layoutBase.ElapsedTime.Value;
                    break;
                case EnumRankingType.Score:
                    userRankingScoreValue = _layoutBase.PlayerTotalDistance.Value;
                    break;
            }

            // 送信
            await PlayFabRankingManager.OnRegisterScoreInRankingAsync(gameRules.RankingDataName, gameRules.RankingType, userRankingScoreValue);
        }
    }
}