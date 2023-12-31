using System;
using UnityEngine;
using FrameworkUnity.OOP.Custom_DI;
using FrameworkUnity.OOP.Interfaces.Listeners;

namespace ShootEmUp
{
    [Serializable]
    public class EnemySpawner : IStartGameListener, IFinishGameListener
    {
        [SerializeField] private GameObject _character;

        private EnemySpawnPool _enemyPool;
        private FixedUpdater _fixedUpdater;
        private EnemyPositions _enemyPositions;
        private EnemiesContainer _enemiesContainer;

        public event Action<GameObject> OnEnemySpawned;
        public event Action<GameObject> OnEnemyUnspawned;


        [Inject]
        public void Construct(
            EnemySpawnPool enemyPool,
            FixedUpdater fixedUpdater,
            EnemyPositions enemyPositions,
            EnemiesContainer enemiesContainer)
        {
            _enemyPool = enemyPool;
            _fixedUpdater = fixedUpdater;
            _enemyPositions = enemyPositions;
            _enemiesContainer = enemiesContainer;
        }

        public void OnStartGame() => OnEnemySpawned += _enemiesContainer.AddActiveEnemy;
        public void OnFinishGame() => OnEnemySpawned -= _enemiesContainer.AddActiveEnemy;

        public void SpawnEnemy()
        {
            if (_enemyPool.TrySpawn(out GameObject enemy))
            {
                Transform spawnPosition = _enemyPositions.RandomSpawnPosition();
                Transform attackPosition = _enemyPositions.GetRandomAttackPosition(enemy);

                enemy.transform.position = spawnPosition.position;
                enemy.GetComponent<EnemyMoveAgent>().SetDestination(attackPosition.position);
                enemy.GetComponent<CircleCollider2DComponent>().SetActiveCollider(false);
                enemy.GetComponent<EnemyInstaller>().Install(_fixedUpdater);

                var agent = enemy.GetComponent<EnemyAttackAgent>();
                agent.SetTarget(_character);
                agent.SetAllowAttack(false);

                OnEnemySpawned?.Invoke(enemy);
            }
        }
        
        public void UnspawnEnemy(GameObject enemy)
        {
            enemy.GetComponent<EnemyInstaller>().Uninstall();

            _enemyPool.Unspawn(enemy);
            _enemyPositions.RestoreAttackPosition(enemy);
            _enemiesContainer.ActiveEnemies.Remove(enemy);

            OnEnemyUnspawned?.Invoke(enemy);
        }
    }
}
