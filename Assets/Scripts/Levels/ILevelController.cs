using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HungwX
{
    public interface ILevelController
    {
        void LoadLevel(int levelID);
        void LoadNextLevel();
        int CurrentLevelID { get; set; }
        List<LevelData> LevelDatas { get; set; }
        GameObject InstanceReference { get; set; }
        AssetReferenceGameObject prefabOfLevel { get; set; }
        void ReleaseLevel();
        void OnAddressableLoadComplete(AsyncOperationHandle<GameObject> handle);
        Action OnLevelLoadComplete { get; set; }
        Action OnLoadNextLevel { get; set; }
    }
}
