using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ranking
{
    /// <summary>
    /// ランキングをUIの表示について管理するクラス
    /// </summary>

    [System.Serializable]
    public class UIRankingViewController
    {
        [SerializeField]
        private UIRankingViewController_RankingElement[] _objectRankingElements;

        [SerializeField]
        private UIRankingViewController_RankingElement _objectUserRank;

        [SerializeField]
        private RectTransform _transformRankingList;

        [SerializeField]
        private ScrollRect _contentRect;

        [SerializeField]
        private RectTransform _contentRectTransform;


        // ユーザーのランク関連
        private bool _isShowUserRank = false;
        private float _scorePlayerData = 0;

        // UIの移動関連
        private float _pointStart;

        private Color colorHigherRank = new Color(221f / 255f, 171f / 255f, 0f / 255f);
        private Color colorPlayerScore = new Color(255f / 255f, 162f / 255f, 186f / 255f);


        public void OnStart()
        {
            _pointStart = _transformRankingList.localPosition.y;
            _transformRankingList.localPosition = new Vector2(0, _pointStart + 1100);
        }

        /// <summary>
        /// ランキング表を設定する
        /// </summary>
        /// <param name="rankingType"></param>
        /// <param name="scores"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public async UniTask OnSetRankingElementsDataAsync(EnumRankingType rankingType,float[] scores, string[] names)
        {
            float playerScore = float.Parse(RankingTypeUtility.OnCheckNotationConversion(rankingType, _scorePlayerData));
            float checkScore = 0;
            for (int i = 0; i < _objectRankingElements.Length; ++i)
            {
                // プレイヤーのスコアと一致する場合は色を変える
                checkScore= float.Parse(RankingTypeUtility.OnCheckNotationConversion(rankingType, scores[i]));

                if (playerScore != 0 && playerScore == checkScore)
                {
                    SetRankElement(_objectRankingElements[i], rankingType, scores[i], names[i], i + 1, true);
                    _scorePlayerData = 0;
                    // 下の段のユーザーのスコアボードの色も変える
                    SetRankElement(_objectUserRank, rankingType, scores[i], names[i], i + 1, true);
                }
                else
                {
                    SetRankElement(_objectRankingElements[i], rankingType, scores[i], names[i], i + 1, false);
                }
            }

            // スコアがNullの部分を非表示にするような処理を作りたかったが、改善できないためコメントアウトして保留
            //// スクロールの設定を初期化する
            //await UniTask.Yield();
            //Canvas.ForceUpdateCanvases(); 
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform); 
            //// ScrollRect の再計算
            //_contentRect.Rebuild(CanvasUpdate.Layout);
            //Canvas.ForceUpdateCanvases();

            _contentRect.verticalNormalizedPosition = 1f;
        }

        /// <summary>
        /// ユーザーのスコアを設定する
        /// </summary>
        /// <param name="score"></param>
        public void OnSetUserRankingData(float score)
        {
            _isShowUserRank = true;
            _scorePlayerData = score;
        }

        //----------------------------------------------------------------
        /// <summary>
        /// スコアを項目に設定する
        /// </summary>
        private void SetRankElement(UIRankingViewController_RankingElement rankElement,EnumRankingType rankingType, float score, string name, int rank, bool isPlayerScore)
        {
            // スコアが0だった場合は消す
            if (score == 0)
            {
                rankElement.SetActive(false);
                return;
            }
            else
            {
                rankElement.SetActive(true);
            }

                rankElement.TextScore.text = RankingTypeUtility.OnCheckNotationConversion(rankingType, score);
            rankElement.TextRank.text = rank.ToString();
            rankElement.TextName.text = name;

            if (isPlayerScore)
                rankElement.OnSetImageScoreBackColor(colorPlayerScore);

            if (rank <= 3)
                rankElement.OnSetImageRankBackColor(colorHigherRank);
        }

        //----------------------------------------------------------------
        /// <summary>
        /// 表示する
        /// </summary>
        /// <param name="time"></param>
        public void OnShowUI(float time)
        {
            // アクティブにする
            _transformRankingList.gameObject.SetActive(true);

            if (_isShowUserRank)
                _objectUserRank.gameObject.SetActive(true);


            // 移動
            _transformRankingList.DOLocalMoveY(_pointStart, time);
        }

        //----------------------------------------------------------------
        /// <summary>
        /// 非表示にする
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public async UniTask OnHideUIAsync(float time)
        {
            // 移動
            _transformRankingList.DOLocalMoveY(_pointStart + 1100, time);

            await UniTask.Delay((int)(time * 1000));

            // 消す
            _transformRankingList.gameObject.SetActive(false);
        }
    }
}