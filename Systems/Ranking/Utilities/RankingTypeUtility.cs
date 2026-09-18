using PlayerAccountSystems;
using UnityEngine;

namespace Ranking
{
    /// <summary>
    /// ランキングデータの加工をするクラス
    /// </summary>
    public static class RankingTypeUtility
    {
        /// <summary>
        /// ランキングの名前がどのタイプに該当するかを調べる
        /// </summary>
        /// <param name="rankingName"></param>
        /// <returns></returns>
        public static EnumRankingType GetRankingType(EnumRankingDataName rankingName)
        {
            EnumRankingType rankingType = EnumRankingType.Time;

            switch (rankingName)
            {
                case EnumRankingDataName.Ranking_EscapeScore:
                case EnumRankingDataName.Ranking_EscapeScore_Exposition1:
                    rankingType = EnumRankingType.Score;
                    break;

                case EnumRankingDataName.Ranking_TimeScoreForest_80:
                case EnumRankingDataName.Ranking_TimeScoreForest_100:
                case EnumRankingDataName.Ranking_TimeScoreForest_120:
                case EnumRankingDataName.Ranking_TimeScoreForest_140:
                    rankingType = EnumRankingType.Time;
                    break;

                default:
                    Debug.LogError("名前が存在しません");
                    break;
            }

            return rankingType;
        }

        /// <summary>
        /// value が referenceValue より優れているかを判定
        /// </summary>
        public static bool IsAIsBetterThanB(EnumRankingType rankingType, float value, float referenceValue)
        {
            bool isBetter = false;

            switch (rankingType)
            {
                case EnumRankingType.Time:
                    isBetter = value < referenceValue;
                    break;

                case EnumRankingType.Score:
                    isBetter = value > referenceValue;
                    break;

                default:
                    Debug.LogError("ランキングタイプが設定されていません");
                    break;
            }

            return isBetter;
        }

        /// <summary>
        /// スコアを対応する表記文字列に変換
        /// </summary>
        public static string OnCheckNotationConversion(EnumRankingType rankingType, float score)
        {
            switch (rankingType)
            {
                case EnumRankingType.Time:
                    decimal d2RoundDown = (decimal)Mathf.Floor(score * 100) / 100;
                    return d2RoundDown.ToString("F2");

                case EnumRankingType.Score:
                    return ((int)score).ToString();

                default:
                    Debug.LogError("ランキングタイプが設定されていません");
                    return score.ToString();
            }
        }
    }
}