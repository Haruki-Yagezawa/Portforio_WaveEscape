using Cysharp.Threading.Tasks;
using GameRules;
using PlayerAccountSystems;
using System;
using UnityEngine;

namespace Ranking
{
    /// <summary>
    /// PlayFabからランキングデータを取得して、ScriptableObjectへ同期するクラス
    /// </summary>
    public class RankingScoreDataSynchronizer : MonoBehaviour
    {
        [SerializeField]
        private RankingScoreData _rankingScoreData;

        /// <summary>
        /// ランキングデータを取得してScriptableObjectを更新する
        /// </summary>
        /// <param name="gameRuleData"></param>
        /// <returns></returns>
        public async UniTask OnSyncRankingDataAsync(GameRulesDataScriptableObject gameRuleData)
        {
            // Nullチェック
            if (gameRuleData == null)
            {
                Debug.LogError("GameRulesDataScriptableObject が null です。");
                return;
            }


            (bool isSuccess, float[] scores, string[] names) rankingDatas = await PlayFabRankingManager.OnGetRankingAsync(gameRuleData.RankingDataName, EnumRankingType.Score);
            if (!rankingDatas.isSuccess)
            {
                Debug.LogWarning ("ランキングデータの取得に失敗しました");
                return;
            }

            int[] intArray = Array.ConvertAll(rankingDatas.scores, score => (int)score);

            _rankingScoreData.OnReset(intArray);
        }
    }
}