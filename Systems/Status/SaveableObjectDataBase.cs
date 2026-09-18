using System;
using UnityEngine;


namespace SaveDataSystem
{
    /// <summary>
    /// セーブデータ
    /// このクラスを継承してセーブしたいデータを追加する
    /// [Serializable]
    /// public class HogeSaveData : SaveDataBase
    /// {
    ///     public int version = 1;
    /// }
    /// </summary>
    [Serializable]
    public abstract class SaveDataBase
    {

        /// <summary>
        /// ローカルのデータを初期化
        /// </summary>
        public virtual void OnInitializeData()
        {
        }
    }

    /// <summary>
    /// セーブ　ロード
    /// </summary>
    public abstract class SaveableObjectDataBase : ScriptableObject
    {

        /// <summary>
        /// データをすべてリセットして保存する
        /// </summary>
        public void InitializeData()
        {
            // ローカルのデータを初期化
            OnInitializeData();

            // セーブデータを初期化
            PlayerPrefs.DeleteKey(SAVE_DATA_KEY_FIRST + SaveDataKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// セーブキー
        /// 継承先で必ず上書きすること
        /// </summary>
        protected virtual string SaveDataKey => "PlayerStatusBase";
        private const string SAVE_DATA_KEY_FIRST = "WaveEscape_Ashiarai_";

        /// <summary>
        /// Prefsに保存
        /// </summary>
        /// <param name="saveData"></param>
        protected void OnSaveToPrefs(SaveDataBase saveData)
        {
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_DATA_KEY_FIRST + SaveDataKey, json);
            PlayerPrefs.Save();
        }

        protected T LoadFromPrefs<T>() where T : SaveDataBase, new()
        {
            string json = PlayerPrefs.GetString(SAVE_DATA_KEY_FIRST + SaveDataKey, "");

            // データが無い → 新規作成
            if (string.IsNullOrEmpty(json))
            {
                T data = new T();
                data.OnInitializeData(); // 初期化処理
                OnSaveToPrefs(data);     // 保存
                return data;
            }

            // JSON → 新しいインスタンスに復元
            T loaded = new T();
            JsonUtility.FromJsonOverwrite(json, loaded);
            return loaded;
        }


        /// <summary>
        /// ローカルのデータを初期化 (継承先で初期化する処理を追加する)
        /// </summary>
        protected abstract void OnInitializeData();

        /// <summary>
        /// Prefsに保存してる情報で上書き (継承先で初期化する処理を追加する)
        /// </summary>
        public abstract void OnLoadToPrefs();
    }
}



//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
// SaveDataBase関数 継承テンプレート
//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

//[Serializable]
//public class SaveData_PlayerStatusData : SaveDataBase
//{
    //public int Level;
    //public int ExperiencePoints;
    //public int Coin;
    //public int SpecialCoin;
    //public int GameProgress;

    ///// <summary>
    ///// 初期化
    ///// </summary>
    //public override void OnInitializeData()
    //{
        //Level = 0;
        //ExperiencePoints = 0;
        //Coin = 0;
        //SpecialCoin = 0;
        //GameProgress = 0;
    //}
//}


//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
// PlayerStatusBase関数 継承テンプレート
//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

////----------------------------------------------------------------------
//// 保存関係
//protected override string SAVE_DATA_KEY => "PlayerStatus";
//private SaveData_PlayerStatusData _saveData = new();

///// <summary>
///// セーブ
///// </summary>
//private void OnSaveToPrefs()
//{
//// データをセーブに反映
//_saveData.Level = _level;
//_saveData.ExperiencePoints = _experiencePoints;
//_saveData.Coin = _coin;
//_saveData.SpecialCoin = _specialCoin;
//_saveData.GameProgress = (int)_gameProgress;

//// セーブ処理
//OnSaveToPrefs(_saveData);
//}

///// <summary>
///// ロード
///// </summary>
//public override void OnLoadToPrefs()
//{
//// セーブ側引出し
//_saveData = LoadFromPrefs<SaveData_PlayerStatusData>();

//// ローカルへ反映
//_level = _saveData.Level;
//_experiencePoints = _saveData.ExperiencePoints;
//_coin = _saveData.Coin;
//_specialCoin = _saveData.SpecialCoin;
//_gameProgress = (EnumGameProgress)_saveData.GameProgress;
//}

///// <summary>
///// 初期化
///// </summary>
//protected override void OnInitializeData()
//{
//// セーブ側初期化
//_saveData.OnInitializeData();

//// ローカル側初期化
//_level = 0;
//_experiencePoints = 0;
//_coin = 0;
//_specialCoin = 0;
//_gameProgress = 0;
//}