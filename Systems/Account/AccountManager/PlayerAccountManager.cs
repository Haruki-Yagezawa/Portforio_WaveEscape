using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayerAccountSystems
{
    /// <summary>
    /// PlayFabアカウントを確認し、ログイン処理を統合管理するクラス
    /// </summary>
    public class PlayerAccountManager : MonoBehaviour
    {
        //===============================================
        // 変数

        // シングルトン
        private static PlayerAccountManager Instance;

        [SerializeField]
        private PlayFabAccountData _accountData;

        [SerializeField]
        private PlayerAccountManager_AskName _askNameUI;

        [Header("UI関連")]
        [SerializeField]
        private UIInfomationManager _loadUI;
        [SerializeField]
        private UIInfomationManager _errorUI;


        private void Awake()
        {
            // シングルトン確認
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //===============================================
        /// <summary>
        /// IDを作るためのユーザーネーム入力
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> OnMakeIDAsync()
        {
            // 名前を入力させるUIを表示
            string name = await _askNameUI.AskNameUIAsync();

            // 空だったら終了
            if (name == "") return false;

            _accountData.DisplayName = name;

            return true;
        }

        /// <summary>
        /// 登録ボタンを押された場合（UnityButtonから呼び出し）
        /// </summary>
        public void OnMakeID_Ask() { _askNameUI.OnAsk(); }

        /// <summary>
        /// キャンセルボタンを押された場合（UnityButtonから呼び出し）
        /// </summary>
        public void OnMakeID_NotAsk() { _askNameUI.OnNotAsk(); }

        //===============================================
        /// <summary>
        /// IDを登録、ログイン
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> OnRegisterAndLogInIDAsync()
        {
            // アカウント作成処理
            if (!await PlayFabLoginManager.Instance.InitializeAsync(_accountData))
            {
                return false;
            }

            return true;
        }


        //===============================================
        //===============================================
        /// <summary>
        /// ログイン確認処理
        /// </summary>
        /// <returns></returns>
        public static async UniTask<bool> CheckLoginAsync()
        {
            // オブジェクトが存在しない場合は終了
            if (Instance == null) return false;

            await Instance._loadUI.SetActive(true);

            // アカウントが作られていない場合は作成する名前を尋ねる
            if (Instance._accountData.LoginId == "")
            {
                if (!await Instance.OnMakeIDAsync())
                {
                    Instance.CheckLoginAsync_ResultError();
                    return false;
                }
            }

            // 登録、ログイン
            if (!await Instance.OnRegisterAndLogInIDAsync())
            {
                Instance.CheckLoginAsync_ResultError();
                return false;
            }

            // 成功
            Instance.CheckLoginAsync_ResultSuccess();
            return true;
        }
        /// <summary>
        /// ログイン確認処理でエラー
        /// </summary>
        private void CheckLoginAsync_ResultError()
        {
            _loadUI.SetActive(false).Forget();

            Debug.Log("エラーです");
            _errorUI.SetActive(true).Forget();

        }
        /// <summary>
        /// ログイン確認処理で成功
        /// </summary>
        private void CheckLoginAsync_ResultSuccess()
        {
            _loadUI.SetActive(false).Forget();
        }

        //===============================================
        //===============================================
        /// <summary>
        /// UI表示
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public static async UniTask OnActiveLoadUI(bool isActive)
        {
            // オブジェクトが存在しない場合は終了
            if (Instance == null) return;

            await Instance._loadUI.SetActive(isActive);
        }
    }
}