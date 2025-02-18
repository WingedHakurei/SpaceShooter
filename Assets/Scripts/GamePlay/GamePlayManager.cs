using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using GamePlay.Runtimes;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using XLua;

namespace GamePlay
{
    // TODO: Asset Bundle
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
        [SerializeField] private string _debugBasicConfigsPath;
        [SerializeField] private string _debugActionsPath;
        [SerializeField] private string _debugEventsPath;
        
        private string BasicConfigsPath => Path.Combine(Application.streamingAssetsPath, _debugBasicConfigsPath + ".lua.txt");
        private string ActionsPath => Path.Combine(Application.streamingAssetsPath, _debugActionsPath + ".lua.txt");
        private string EventsPath => Path.Combine(Application.streamingAssetsPath, _debugEventsPath + ".lua.txt");

        private async void Start()
        {
            var destroyToken = this.GetCancellationTokenOnDestroy();
            _luaEnv = new LuaEnv();
            Pool<Trigger2D>.Instance = new Pool<Trigger2D>(_debugPrefabNames, _debugPrefabs);
            
            var (configs, loadSuccess) = await LoadConfigsAsync(BasicConfigsPath, ActionsPath, EventsPath, destroyToken);
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
            _entities.SetActionInvoker(_controllers.eventController.InvokeAction);
            _luaEnv.Global.Set<string, Action<string, RuntimeBase>>("InvokeEvent", _controllers.eventController.InvokeEvent);
        }

        private async UniTask<(GamePlayConfigs, bool)> LoadConfigsAsync(string basicPath, string actionsPath, string eventsPath, CancellationToken token = default)
        {
            var basicReq = UnityWebRequest.Get(basicPath);
            var actionsReq = UnityWebRequest.Get(actionsPath);
            var eventsReq = UnityWebRequest.Get(eventsPath);
            await UniTask.WhenAll(
                basicReq.SendWebRequest().WithCancellation(token), 
                actionsReq.SendWebRequest().WithCancellation(token), 
                eventsReq.SendWebRequest().WithCancellation(token));
            
            if (basicReq.result != UnityWebRequest.Result.Success ||
                actionsReq.result != UnityWebRequest.Result.Success ||
                eventsReq.result != UnityWebRequest.Result.Success)
            {
                return (null, false);
            }
            
            var basicLua = _luaEnv.DoString(basicReq.downloadHandler.data)[0] as LuaTable;
            var luaActions = _luaEnv.DoString(actionsReq.downloadHandler.data)[0] as LuaTable;
            var luaEvents = _luaEnv.DoString(eventsReq.downloadHandler.data)[0] as LuaTable;

            var configs = new GamePlayConfigs(basicLua, luaActions, luaEvents);

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
            _controllers.Dispose();
            _luaEnv.Dispose();
            Pool<Trigger2D>.Instance.Dispose();
        }
    }
}