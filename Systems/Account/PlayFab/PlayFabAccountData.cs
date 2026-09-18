using SaveDataSystem;
using System;
using UnityEngine;

namespace PlayerAccountSystems
{
    [Serializable]
    public class SaveData_PlayFabAccountData : SaveDataBase
    {
        public string LoginId;
        public string UserId;
        public string DisplayName;

        /// <summary>
        /// 初期化
        /// </summary>
        public override void OnInitializeData()
        {
            LoginId = "";
            UserId = "";
            DisplayName = "";
        }
    }

    /// <summary>
    /// PlayFabアカウント関連データ
    /// </summary>
    [CreateAssetMenu(fileName = "PlayFabAccount", menuName = "ScriptableObject/PlayFab/AccountData")]
    public class PlayFabAccountData : SaveableObjectDataBase
    {
        // ログイン用のカスタムID
        [SerializeField]
        private string _loginId;

        // 表示用のユーザーID
        [SerializeField]
        private string _userId;

        // ユーザーの表示名
        [SerializeField]
        private string _displayName;


        public string LoginId
        {
            get { return _loginId; }
            set { if (_loginId != value) {_loginId = value; OnSaveToPrefs(); } }
        }

        public string UserId
        {
            get { return _userId; }
            set { if (_userId != value) { _userId = value; OnSaveToPrefs(); } }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { if (_displayName != value) { _displayName = value; OnSaveToPrefs(); } }
        }


        //----------------------------------------------------------------------
        // 保存関係
        protected override string SaveDataKey => "PlayFabAccount";
        private SaveData_PlayFabAccountData _saveData = new();

        /// <summary>
        /// セーブ
        /// </summary>
        private void OnSaveToPrefs()
        {
            // データをセーブに反映
            _saveData.LoginId = _loginId;
            _saveData.UserId = _userId;
            _saveData.DisplayName = _displayName;

            // セーブ処理
            OnSaveToPrefs(_saveData);
        }

        /// <summary>
        /// ロード
        /// </summary>
        public override void OnLoadToPrefs()
        {
            // セーブ側引出し
            _saveData = LoadFromPrefs<SaveData_PlayFabAccountData>();
            
            // ローカルへ反映
            _loginId = _saveData.LoginId;
            _userId = _saveData.UserId;
            _displayName = _saveData.DisplayName;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void OnInitializeData()
        {
            // セーブ側初期化
            _saveData.OnInitializeData();

            // ローカル側初期化
            _loginId = "";
            _userId = "";
            _displayName = "";
        }
    }
}