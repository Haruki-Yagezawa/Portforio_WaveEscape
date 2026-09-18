using UnityEngine;
using UnityEngine.UI;

namespace Ranking
{
    /// <summary>
    /// ランキングUIに並ぶそれぞれのランキング行を管理するクラス
    /// </summary>
    public class UIRankingViewController_RankingElement : MonoBehaviour
    {
        [SerializeField] private Text _textScore;
        [SerializeField] private Image _imageScoreBack;
        [SerializeField] private Text _textRank;
        [SerializeField] private Image _imageRankBack;
        [SerializeField] private Text _textName;

        public Text TextScore => _textScore;
        public Text TextRank => _textRank;
        public Text TextName => _textName;

        public void SetActive(bool isBool)
        {
            gameObject.SetActive(isBool);
        }

        public void OnSetImageScoreBackColor(Color color)
        {
            _imageScoreBack.color = color;
        }

        public void OnSetImageRankBackColor(Color color)
        {
            _imageRankBack.color = color;
        }
    }
}