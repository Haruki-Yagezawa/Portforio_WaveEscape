using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Ranking;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAccountSystems
{
    /// <summary>
    /// PlayFabランキング機能関連のクラス
    /// </summary>
    public class PlayFabRankingManager
    {
        //-----------------------------------------------------------------
        /// <summary>
        /// スコアを送信
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="rankingType"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static async UniTask OnRegisterScoreInRankingAsync(EnumRankingDataName dataName, EnumRankingType rankingType, float score)
        {
            UniTaskCompletionSource utcs = new();

            // スコアを * 1000 する
            int intScore = (int)(score * 1000);

            int modifiedScore = intScore;

            // タイムを表示する場合は値を変換する
            if (rankingType == EnumRankingType.Time)
                modifiedScore = int.MaxValue - intScore;

            // 送信する内容を作る
            var statisticUpdate = new StatisticUpdate
            {
                StatisticName = dataName.ToString(),
                Value = modifiedScore,
            };

            // 作った内容をリクエストにまとめる
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> { statisticUpdate }
            };

            // PlayFabへのログイン処理
            if (!await PlayerAccountManager.CheckLoginAsync())
            {
                return;
            }
            
            // リクエストをPlayFabに送信する
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnSubmitScoreSuccess, OnSubmitScoreFailure);

            // 送信成功時の処理
            void OnSubmitScoreSuccess(UpdatePlayerStatisticsResult result)
            {
                Debug.Log("スコアの送信に成功しました");
                utcs.TrySetResult();
            }

            // 送信失敗時の処理
            void OnSubmitScoreFailure(PlayFabError error)
            {
                Debug.LogError("スコアの送信に失敗しました: " + error.GenerateErrorReport());
                utcs.TrySetResult();
            }


            // リクエストの返答を待つ
            await utcs.Task;

            return;
        }

        //-----------------------------------------------------------------
        /// <summary>
        /// ランキングを取得
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="rankingType"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, float[] scores, string[] names)> OnGetRankingAsync(EnumRankingDataName dataName, EnumRankingType rankingType)
        {
            UniTaskCompletionSource<(bool isSuccess, float[] scores, string[] names)> utcs = new();

            // PlayFabに送信するリクエストを作成する
            var request = new GetLeaderboardRequest
            {
                StatisticName = dataName.ToString(),
                StartPosition = 0,
                MaxResultsCount = 100
            };

            // PlayFabにリクエストを送信する
            if(!await PlayerAccountManager.CheckLoginAsync())
            {
                return (false, null, null);
            }

            PlayFabClientAPI.GetLeaderboard(request, OnGetRankingSuccess, OnGetRankingFailure);

            // 送信成功時の処理
            void OnGetRankingSuccess(GetLeaderboardResult leaderboardResult)
            {
                float[] scores = new float[100];
                string[] names = new string[100];

                for (int i=0;i<leaderboardResult.Leaderboard.Count;++i)
                {
                    // ユーザー名を設定
                    string name = leaderboardResult.Leaderboard[i].DisplayName;
                    names[i] = name;

                    // スコアを設定
                    int modifiedScore = leaderboardResult.Leaderboard[i].StatValue;
                    scores[i] = modifiedScore;
                    // タイムを表示する場合は値を変換する
                    if (rankingType == EnumRankingType.Time)
                        scores[i] = int.MaxValue - modifiedScore;
                    scores[i] /= 1000;
                }

                utcs.TrySetResult((true, scores, names));
            }

            // 送信失敗時の処理
            void OnGetRankingFailure(PlayFabError error)
            {
                Debug.Log("ランキングの取得に失敗しました");

                utcs.TrySetResult((false, null, null));
            }

            // リクエストの返答を待つ
            return await utcs.Task;
        }



        //-----------------------------------------------------------------
        /// <summary>
        /// プレイヤーのスコアを取得する
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="rankingType"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, float score)> OnGetPlayerScore(EnumRankingDataName dataName, EnumRankingType rankingType)
        {
            float score = 0;
            UniTaskCompletionSource<(bool isSuccess, float score)> utcs = new();

            // PlayFabに送信するリクエストを作成する
            var request = new GetPlayerStatisticsRequest
            {
                StatisticNames = new List<string> { dataName.ToString() }
            };

            // PlayFabにリクエストを送信する
            if (!await PlayerAccountManager.CheckLoginAsync())
            {
                return (false, score);
            }

            PlayFabClientAPI.GetPlayerStatistics(request, OnGetScoreSuccess, OnGetScoreFailure);

            // 送信成功時の処理
            void OnGetScoreSuccess(GetPlayerStatisticsResult result)
            {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == dataName.ToString())
                    {
                        int raw = stat.Value;

                        if (rankingType == EnumRankingType.Time)
                            raw = int.MaxValue - raw;

                        score = raw / 1000f; break;
                    }
                }

                utcs.TrySetResult((true, score));
            }

            // 送信失敗時の処理
            void OnGetScoreFailure(PlayFabError error)
            {
                Debug.Log("スコアの取得に失敗しました");

                utcs.TrySetResult((false, score));
            }

            // リクエストの返答を待つ
            return await utcs.Task;
        }

    }
}