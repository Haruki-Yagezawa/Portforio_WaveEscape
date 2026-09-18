using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace PlayerAccountSystems
{
    /// <summary>
    /// PlayFabのログイン用クラス
    /// </summary>
    public class PlayFabLoginManager
    {
        private static PlayFabLoginManager _instance;

        /// <summary>
        /// シングルトンのインスタンスにアクセス
        /// </summary>
        public static PlayFabLoginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayFabLoginManager();
                }
                return _instance;
            }
        }

        private PlayFabLoginManager()
        {
            // 初期化処理（例えば、必要に応じて何か設定する場合）
        }

        /// <summary>
        /// PlayFabにログインする
        /// </summary>
        /// <param name="accountData"></param>
        /// <returns></returns>
        public async UniTask<bool> InitializeAsync(PlayFabAccountData accountData)
        {
            // ログインされていてセッションも続いていたら終了
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                return true;
            }

            if (!accountData)
            {
                Debug.LogError("アカウントデータがNullです");
                return false;
            }

            if (string.IsNullOrEmpty(accountData.LoginId))
            {
                Debug.Log("LoginIdが取得できませんでした。\n新しいアカウントを登録します");
                if (!await CreateNewAccountAsync(accountData))
                    return false;
            }
            else
            {
                if (!await LoginWithSavedAccountAsync(accountData))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 新しいアカウントを登録する
        /// </summary>
        /// <param name="accountData"></param>
        /// <returns></returns>
        private async UniTask<bool> CreateNewAccountAsync(PlayFabAccountData accountData)
        {
            string newLoginId = SystemInfo.deviceUniqueIdentifier;
            // 展示会用にはこっちに切り替える
            // string newLoginId = System.Guid.NewGuid().ToString();

            accountData.LoginId = newLoginId;

            // パケットを作成
            var request = new LoginWithCustomIDRequest
            {
                CustomId = newLoginId,
                CreateAccount = true
            };

            bool isSuccess = false;
            bool isReply = false;

            // 通信開始
            PlayFabClientAPI.LoginWithCustomID(request,
                async result =>
                {
                    // Debug.Log("新しく作ったアカウントで正常にログイン！");

                    //ーーーーーーーーーーーーーーーーーーー
                    // 初期名を設定

                    // 送受信、
                    if (!await PlayFabDisplayNameManager.SetDisplayName_FromAccountData(accountData))
                    {
                        Debug.LogError("名前の設定に失敗した");

                        isSuccess = false;
                        isReply = true;
                    }
                    else
                    {

                        //ーーーーーーーーーーーーーーーーーーー
                        // 表示用のユーザーIDを取得
                        accountData.UserId = result.PlayFabId;

                        SaveAccountData(accountData);
 
                        isSuccess = true;
                        isReply = true;
                    }
                },
                error =>
                {
                    Debug.LogError("ログインに失敗： " + error.ErrorMessage);

                    accountData.DisplayName = "user";
                    SaveAccountData(accountData);

                    isSuccess = false;
                    isReply = true;
                });

            // 通信の返答が帰ってくるまで待機
            await UniTask.WaitUntil(() => isReply);

            return isSuccess;
        }

        /// <summary>
        /// アカウントにログイン
        /// </summary>
        /// <param name="accountData"></param>
        /// <returns></returns>
        private async UniTask<bool> LoginWithSavedAccountAsync(PlayFabAccountData accountData)
        {
            // パケットを作成
            var request = new LoginWithCustomIDRequest
            {
                CustomId = accountData.LoginId,
                CreateAccount = false
            };

            bool isSuccess = false;
            bool isReply = false;

            // 通信開始
            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                    // Debug.Log("既存のアカウントで正常にログイン！");

                    isSuccess = true;
                    isReply = true;
                },
                error =>
                {
                    Debug.LogError("ログインに失敗： " + error.ErrorMessage);
                    isSuccess = false;
                    isReply = true;
                });

            // 通信の返答が帰ってくるまで待機
            await UniTask.WaitUntil(() => isReply);

            return isSuccess;
        }

        private void SaveAccountData(PlayFabAccountData accountData)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(accountData);
#endif
        }
    }
}