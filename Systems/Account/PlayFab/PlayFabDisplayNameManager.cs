using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace PlayerAccountSystems
{
    /// <summary>
    /// PlayFabの表示名の設定・取得するヘルパークラス
    /// </summary>
    public class PlayFabDisplayNameManager
    {
        /// <summary>
        /// ユーザーの名前を設定
        /// </summary>
        /// <param name="accountData"></param>
        /// <returns></returns>
        public static async UniTask<bool> SetDisplayName_FromAccountData(PlayFabAccountData accountData)
        {
            UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = accountData.DisplayName
            };

            return await SetPlayerProfile(request);
        }

        /// <summary>
        /// ユーザーの名前を設定（パケットを事前作成）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async UniTask<bool> SetPlayerProfile(UpdateUserTitleDisplayNameRequest request)
        {
            UniTaskCompletionSource<bool> utcs = new();

            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                result => {
                    // 成功
                    utcs.TrySetResult(true);
                },
                error => {
                    // 失敗
                    Debug.LogError($"エラー: {error.GenerateErrorReport()}");
                    utcs.TrySetResult(false);

                });

            // リクエストの返答を待つ
            return await utcs.Task;
        }

        //========================================================================================
        /// <summary>
        /// ユーザーの名前を取得
        /// </summary>
        /// <param name="accountData"></param>
        /// <returns></returns>
        public static async UniTask<bool> GetDisplayName(PlayFabAccountData accountData)
        {
            bool isSuccess = false;

            // リクエストデータ
            GetPlayerProfileRequest request = new GetPlayerProfileRequest
            {
                PlayFabId = accountData.LoginId,
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true
                }
            };

            // リクエスト送信
            var profile = await GetPlayerProfile(request);
            if (profile != null)
            {
                // 成功した場合の処理
                accountData.DisplayName = profile.DisplayName;
                isSuccess = true;
            }

            return isSuccess;
        }

        /// <summary>
        /// ユーザーの名前を取得（パケットを事前作成）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async UniTask<PlayerProfileModel> GetPlayerProfile(GetPlayerProfileRequest request)
        {
            UniTaskCompletionSource<PlayerProfileModel> utcs = new();


            PlayFabClientAPI.GetPlayerProfile(request,
                result =>
                {
                    // 成功
                    utcs.TrySetResult(result.PlayerProfile);
                },
                error =>
                {
                    // 失敗
                    Debug.LogError($"エラー:取得失敗\n {error.GenerateErrorReport()}");
                    utcs.TrySetResult(null);
                });

            // リクエストの返答を待つ
            return await utcs.Task;
        }

    }
}