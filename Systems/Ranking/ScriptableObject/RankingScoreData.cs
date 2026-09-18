using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ranking
{
    /// <summary>
    /// ランキングの実体クラス
    /// </summary>
    [System.Serializable]
    public class RankingScores
    {
        [SerializeField]
        private List<int> _scores = new();

        [SerializeField]
        private int _rankingLength = 100;

        public int RankMax => _rankingLength;

        /// <summary>
        /// スコアを追加
        /// </summary>
        public void AddScore(int score)
        {
            // 二分探索で挿入すべきインデックスを探す
            int index = _scores.BinarySearch(score, Comparer<int>.Create((a, b) => b.CompareTo(a)));

            // 見つからない場合は挿入位置を取得
            if (index < 0) index = ~index;


            // ランク外なら追加しない
            if (index >= _rankingLength) return;

            // 適切な位置に挿入
            _scores.Insert(index, score);

            // 上限を超えた分を切り捨てる
            if (_scores.Count > _rankingLength)
            {
                _scores.RemoveAt(_scores.Count - 1);
            }
        }

        public void AddScore(IEnumerable<int> scores)
        {
            foreach (var score in scores)
            {
                AddScore(score);
            }
        }

        /// <summary>
        /// 指定スコアのランキング順位を取得（1始まり）
        /// </summary>
        public int GetRankToScore(int score)
        {
            int index = _scores.BinarySearch(score, Comparer<int>.Create((a, b) => b.CompareTo(a)));
            if (index < 0) index = ~index;

            // 1始まりの順位として返し、最大件数内に収める
            return Mathf.Clamp(index + 1, 1, _rankingLength);
        }

        /// <summary>
        /// 指定順位のスコアを取得（1始まり）
        /// </summary>
        public int GetScore(int rank)
        {
            int index = rank - 1;

            if (index < 0 || index >= _scores.Count)
            {
                // データがない順位の場合は0を返す
                return 0;
            }

            return _scores[index];
        }
    }

    /// <summary>
    /// ランキングの実体を保持するクラス
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Systems/Ranking/ScoreData", fileName = "RankingScoreData")]
    public class RankingScoreData : ScriptableObject
    {
        [SerializeField]
        private RankingScores _scores;

        public int RankMax => _scores.RankMax;

        public void OnReset(int[] rankingDatas)
        {
            _scores = new();
            _scores.AddScore(rankingDatas);
        }

        public void GetScoreRankWithNeighbors(int score, out int rank, out int up_Score, out int down_Score)
        {
            rank = _scores.GetRankToScore(score);

            // 前後のスコアを取得
            up_Score = rank > 1 ? _scores.GetScore(rank - 1) : 0;
            down_Score = rank < _scores.RankMax ? _scores.GetScore(rank) : 0;
        }
    }
}