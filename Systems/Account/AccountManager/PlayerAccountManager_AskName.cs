using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerAccountSystems
{
    /// <summary>
    /// ユーザーの名前を尋ね、正常に登録したかを確認するクラス
    /// </summary>
    [System.Serializable]
    public class PlayerAccountManager_AskName
    {
        [SerializeField]
        private UIInfomationManager _askNameUI;

        [SerializeField]
        private InputField _nameText;

        private bool _isSuccess = false;
        private bool _isReply = false;

        /// <summary>
        /// UIを表示して入力待機
        /// </summary>
        /// <returns></returns>
        public async UniTask<string> AskNameUIAsync()
        {
            // 初期化
            _isReply = false;
            _nameText.text = "";


            // UI表示
            await _askNameUI.SetActive(true);


            // 返答があるまで待機
            await UniTask.WaitUntil(() => _isReply);

            // UI非表示
            await _askNameUI.SetActive(false);

            if (_isSuccess)
            {
                // 文字がフォーマットにあっていない場合異常終了
                if (_nameText.text.Length < 3) return "";

                // 名前登録ボタンを押していたらその名前を返す
                return _nameText.text;
            }
            else
            {
                // バックボタンを押していたら異常終了
                return "";
            }

        }

        /// <summary>
        /// 登録！！
        /// </summary>
        public void OnAsk()
        {
            _isSuccess = true;
            _isReply = true;

        }

        /// <summary>
        /// 登録しない
        /// </summary>
        public void OnNotAsk()
        {
            _isSuccess = false;
            _isReply = true;
        }
    }
}