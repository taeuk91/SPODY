using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid): base(guid) {}
}

public class AddressableManager : Singleton<AddressableManager>
{
    [SerializeField]
    private AssetReference playerAssetReference;
    
    [SerializeField]
    private AssetReferenceAudioClip musicAssetRefernce;

    // Level Loading
    private bool clearPreviewScene = false;
    private SceneInstance previousLoadedScene;
    public string nowScene;
    public string previousScene;

    // private void Start()
    // {
    //     //Addressables.InitializeAsync();
    //     //Loader.instance.StartLoader();
    //     print("Initialized Addressables...");
    //     Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    // }

    public void Init()
    {
        //Loader.instance.StartLoader();
        if (playerAssetReference == null)
        {
            playerAssetReference = new AssetReference();
            //musicAssetRefernce = new AssetReferenceAudioClip();
            
            print("Initialized Addressables...");
            Addressables.InitializeAsync().Completed += AddressableManager_Completed;
        }
    }
    
    private GameObject playerGo;
    private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        Debug.Log("Initializing Addressables...");
        
        // Load Player Asset
        playerAssetReference.LoadAssetAsync<GameObject>().Completed +=(playerAsset) =>
        {
            Debug.Log("Loading the player...");

            // Instantiate Player Asset
            playerAssetReference.InstantiateAsync().Completed += (playerAssetGO) =>
            {
                Debug.Log("Instantiating the player...");

                playerGo = playerAssetGO.Result;
                Debug.Log("Instantiated the player...");

            };
        };
        
        // 게임 오브젝트 로드
        // playerAssetReference.InstantiateAsync().Completed += (go) =>
        // {
        //     playerGo = go.Result;
        //     Debug.Log("Instantiated the player...");
        //
        // };

        // 음악 로드
        // musicAssetRefernce.LoadAssetAsync().Completed += (clip) =>
        // {
        //     var audioSource = gameObject.AddComponent<AudioSource>();
        //     audioSource.clip = clip.Result;
        //     audioSource.playOnAwake = false;
        //     audioSource.Play();
        //     
        //     Debug.Log("Loaded the audio clip...");
        //
        // };
        
        Debug.Log("Loaded Asset...");
    }


    public void LoadAddressableLevel(string addressbleKey)
    {
        if (clearPreviewScene)
        {
            Addressables.UnloadSceneAsync(previousLoadedScene).Completed += (asyncHandle) =>
            {
                clearPreviewScene = false;
                previousLoadedScene = new SceneInstance();
                Debug.Log($"Unloaded Scene {addressbleKey} successfully");
            };
        }

        // LoadSceneMode.Single(씬 넘어가기) or LoadSceneMode.additive(씬 추가)
        Addressables.LoadSceneAsync(addressbleKey, LoadSceneMode.Single).Completed += (asyncHandle) =>
        {
            clearPreviewScene = true;
            previousLoadedScene = asyncHandle.Result;
            nowScene = addressbleKey;
            Debug.Log($"Loaded Scene {addressbleKey} successfully");
        };
    }
    

    // private void Update()
    // {
    //     // Checking for loading content completion
    //     if (playerAssetReference.Asset != null && musicAssetRefernce.Asset != null && Loader.instance.IsLoading)
    //     {
    //         // To do : 
    //         Loader.instance.StopLoader();
    //     }
    // }
    
    

    private void OnDestroy()
    {
        playerAssetReference.ReleaseInstance(playerGo);
    }
}
