using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using GamePlay.Runtimes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using XLua;

namespace GamePlay
{
    public class GamePlayManager : MonoBehaviour
    {
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        private GamePlayControllers _controllers;
        private int _gameSpeed;
        private const int MaxGameSpeed = 5;
        private bool _isInitialized;
        
        private LuaEnv _luaEnv;
        
        [SerializeField, Range(0, MaxGameSpeed)] private int _debugGameSpeed;
        [SerializeField] private AssetReference[] _prefabRefs;
        [SerializeField] private AssetReference _configsRef;
        [SerializeField] private AssetReference _actionsRef;
        [SerializeField] private AssetReference _eventsRef;

        private async void Start()
        {
            var destroyToken = this.GetCancellationTokenOnDestroy();
            _luaEnv = new LuaEnv();

            var (prefabs, configs) =
                await UniTask.WhenAll(LoadPrefabsAsync(destroyToken), LoadConfigsAsync(destroyToken));
            Pool<Trigger2D>.Instance = new Pool<Trigger2D>(prefabs);
            _configs = configs;

            _entities = new GamePlayEntities();
            _controllers = new GamePlayControllers(_configs);
            _entities.SetActionInvoker(_controllers.eventController.InvokeAction);
            _luaEnv.Global.Set<string, Action<string, RuntimeBase>>("InvokeEvent", _controllers.eventController.InvokeEvent);
            _isInitialized = true;
        }

        private async UniTask<Trigger2D[]> LoadPrefabsAsync(CancellationToken token = default)
        {
            var gos = await UniTask.WhenAll(_prefabRefs.Select(pr => pr.LoadAssetAsync<GameObject>().WithCancellation(token)));
            return gos.Select(go => go.GetComponent<Trigger2D>()).Where(t2d => t2d).ToArray();
        }

        private async UniTask<GamePlayConfigs> LoadConfigsAsync(CancellationToken token = default)
        {
            var (basicLuaText, luaActionsText, luaEventsText) = await UniTask.WhenAll(
                _configsRef.LoadAssetAsync<TextAsset>().WithCancellation(token), 
                _actionsRef.LoadAssetAsync<TextAsset>().WithCancellation(token), 
                _eventsRef.LoadAssetAsync<TextAsset>().WithCancellation(token));
            
            var basicLua = basicLuaText ? _luaEnv.DoString(basicLuaText.bytes)[0] as LuaTable : null;
            var luaActions = luaActionsText ? _luaEnv.DoString(luaActionsText.bytes)[0] as LuaTable : null;
            var luaEvents = luaEventsText ? _luaEnv.DoString(luaEventsText.bytes)[0] as LuaTable : null;
            
            Addressables.Release(basicLuaText);
            Addressables.Release(luaActionsText);
            Addressables.Release(luaEventsText);

            return new GamePlayConfigs(basicLua, luaActions, luaEvents);
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            _gameSpeed = _debugGameSpeed;
            var delta = Time.deltaTime * _gameSpeed / MaxGameSpeed;

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