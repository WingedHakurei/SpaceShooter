using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using XLua;

namespace GamePlay
{
    // TODO: xLua Events
    // TODO: Save & Load
    public class GamePlayManager : MonoBehaviour
    {
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        private GamePlayControllers _controllers;
        private int _gameSpeed;
        private float _gameTime;
        private const int MaxGameSpeed = 5;
        
        private LuaEnv _luaEnv;
        
        [SerializeField, Range(0, MaxGameSpeed)] private int _debugGameSpeed;
        [SerializeField] private string[] _debugPrefabNames;
        [SerializeField] private Trigger2D[] _debugPrefabs;
        [SerializeField] private string _debugConfigsPath;

        private async void Start()
        {
            var destroyToken = this.GetCancellationTokenOnDestroy();
            _luaEnv = new LuaEnv();
            Pool<Trigger2D>.Instance = new Pool<Trigger2D>(_debugPrefabNames, _debugPrefabs);

            var fullConfigPath = Path.Combine(Application.streamingAssetsPath, _debugConfigsPath + ".lua.txt");
            var (configs, loadSuccess) = await LoadConfigsAsync(fullConfigPath, destroyToken);
            if (!loadSuccess)
            {
                #if UNITY_EDITOR
                Debug.LogError("Load configs failed");
                #else
                Application.Quit();
                #endif
            }
            _configs = configs;

            _entities = new GamePlayEntities();
            _controllers = new GamePlayControllers(_configs);
            
        }

        private async UniTask<(GamePlayConfigs, bool)> LoadConfigsAsync(string path, CancellationToken token = default)
        {
            var request = await UnityWebRequest.Get(path).SendWebRequest().WithCancellation(token);
            if (request.result != UnityWebRequest.Result.Success)
            {
                return (null, false);
            }
            var luaData = request.downloadHandler.data;
            var luaConfigs = _luaEnv.DoString(luaData)[0] as LuaTable;
            var configs = new GamePlayConfigs(luaConfigs);

            return (configs, true);
        }

        private void Update()
        {
            _gameSpeed = _debugGameSpeed;
            var delta = Time.deltaTime * _gameSpeed / MaxGameSpeed;
            _gameTime += delta;

            _controllers.Update(delta);
            
            foreach (var fighter in _entities.fighters.Values)
            {
                fighter.Update(delta);
            }

            foreach (var bullet in _entities.bullets.Values)
            {
                bullet.Update(delta);
            }
        }

        private void OnDestroy()
        {
            Pool<Trigger2D>.Instance.Dispose();
            _luaEnv.Dispose();
        }
    }
}