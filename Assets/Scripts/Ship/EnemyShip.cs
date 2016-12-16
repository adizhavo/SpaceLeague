using UnityEngine;
using SpaceLeague.Ship;
using SpaceLeague.Ship.Weapon;

namespace SpaceLeague.Ship.Enemy
{
    public class EnemyShip : AbstractShip 
    {
        [SerializeField] private Vector2 distanceFromPlayer;
        [SerializeField] private Vector2 freeRoamingTime;
        [SerializeField] private Vector2 journeyTime;
        [SerializeField] private Vector2 battleTime;
        [SerializeField] private ShipMainCannon mainCannon;

        private Transform playerShip;
        private Vector3 currentTargetPoint;
        private Vector3 targetPoint;

        private float freeRoamingTickCounter;
        private float journeyTickCounter;
        private float battleTickCounter;

        private float pickedBattleTime;
        private float pickedFreeRoamingTime;
        private float pickedJourneyTime;

        private Transform targetShip;

        private enum FlyModes
        {
            FreeRoaming,
            Battle
        }
        private FlyModes currentMode;

        private void Start()
        {
            playerShip = GameObject.FindGameObjectWithTag("Player").transform;
            currentTargetPoint = ship.position;
            EnterFreeRoamingMode();
        }

        private void ChooseJourneyPosition()
        {
            pickedJourneyTime = Random.Range(journeyTime.x, journeyTime.y);
            targetPoint = playerShip.position + Random.onUnitSphere * Random.Range(distanceFromPlayer.x, distanceFromPlayer.y);
            journeyTickCounter = 0f;
        }

        protected override void Update()
        {
            base.Update();

            UpdateTargetPoint();
            UpdateTimers();
            CalculateAimDirection(currentTargetPoint, 2f); // this magic number can be the difficulty of the AI
            UpdateShipDirection();
            TryToFire();

            #if UNITY_EDITOR
            DisplayLogs();
            #endif
        }

        private void FixedUpdate()
        {
            MoveShipForward();
        }

        private void TryToFire()
        {
            if (targetShip != null && ShipUtils.IsTargetOnSight(targetShip, ship, GlobalDirection)) mainCannon.OpenFire();
        }

        private void UpdateFlyMode()
        {
            if (targetShip == null) currentMode = FlyModes.FreeRoaming;
        }

        private void UpdateTargetPoint()
        {
            if (targetShip != null) currentTargetPoint = targetShip.position;
            else currentTargetPoint = Vector3.Lerp(currentTargetPoint, targetPoint, Time.deltaTime);
        }

        private void UpdateTimers()
        {
            if (currentMode.Equals(FlyModes.FreeRoaming))
            {
                journeyTickCounter += Time.deltaTime / pickedJourneyTime;
                if (journeyTickCounter > 1f) ChooseJourneyPosition();

                freeRoamingTickCounter += Time.deltaTime / pickedFreeRoamingTime;
                if (freeRoamingTickCounter > 1f) 
                {
                    Transform t = ShipUtils.ChooseTargetToFightForAI(this);
                    if (t != null) EnterBattleMode(t);
                }
            }
            else if (currentMode.Equals(FlyModes.Battle))
            {
                battleTickCounter += Time.deltaTime / pickedBattleTime;
                if (battleTickCounter > 1f) EnterFreeRoamingMode();
            }
        }

        protected override void DisplayLogs()
        {
            base.DisplayLogs();
            Debug.DrawLine(transform.position, currentTargetPoint, Color.cyan);
            Debug.DrawLine(transform.position, targetPoint, Color.yellow);
        }

        public override void Damage(Transform attackingShip, float damage)
        {
            EnterBattleMode(attackingShip);

            #if UNITY_EDITOR
            Debug.Log("Enemy damaged by " + attackingShip.name);
            #endif

            base.Damage(attackingShip, damage);
        }

        private void EnterFreeRoamingMode()
        {
            #if UNITY_EDITOR
            Debug.Log("Enemy enters battle free roaming mode");
            #endif

            targetShip = null;
            currentMode = FlyModes.FreeRoaming;
            pickedFreeRoamingTime = Random.Range(freeRoamingTime.x, freeRoamingTime.y);
            freeRoamingTickCounter = 0f;
            ChooseJourneyPosition();
        }

        private void EnterBattleMode(Transform attackingShip)
        {
            #if UNITY_EDITOR
            Debug.Log("Enemy enters battle mode " + attackingShip.name);
            #endif

            targetShip = attackingShip;
            currentMode = FlyModes.Battle;
            pickedBattleTime = Random.Range(battleTime.x, battleTime.y);
            battleTickCounter = 0f;
        }
    }
}
