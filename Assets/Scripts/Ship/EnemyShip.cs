using UnityEngine;
using SpaceLeague.Ship;
using SpaceLeague.Ship.Weapon;

namespace SpaceLeague.Ship.Enemy
{
    public class EnemyShip : Ship 
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
            TryToFire();

            #if UNITY_EDITOR
            DisplayLogs();
            #endif
        }

        private void FixedUpdate()
        {
            UpdateShipDirection();
            MoveShipForward();
        }

        private void TryToFire()
        {
            if (targetShip != null && IsTargetOnSight()) mainCannon.OpenFire();
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
                if (freeRoamingTickCounter > 1f) ChooseTargetToFight();
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

        protected bool IsTargetOnSight()
        {
            if (targetShip == null) return false;

            Vector3 targetProjection = Vector3.ProjectOnPlane(targetShip.position - ship.position, ship.forward);
            Vector2 targetLocalProjection = ship.InverseTransformPoint(targetProjection);
            Vector3 globalDirectionProjection = Vector3.ProjectOnPlane(GlobalDirection - ship.position, ship.forward);
            Vector2 globalDirectionLocalProjection = ship.InverseTransformPoint(globalDirectionProjection);

            Rect aimRect = new Rect(
                globalDirectionLocalProjection.x - 3f,
                globalDirectionLocalProjection.y - 3f, 
                6f, 
                6f);

            bool isInsideRect = aimRect.Contains(targetLocalProjection);

            Vector3 targetVerticalProjection = Vector3.ProjectOnPlane(targetShip.position - ship.position, ship.up);
            Vector3 targetVerticalLocalProjection = ship.InverseTransformPoint(ship.position + targetVerticalProjection);
            int sign = (int)Mathf.Sign(targetVerticalLocalProjection.z);
            return sign > 0 && isInsideRect;
        }

        public override void Damaged(Transform attackingShip, float damage)
        {
            #if UNITY_EDITOR
            Debug.Log("Enemy damaged by " + attackingShip.name);
            #endif
            EnterBattleMode(attackingShip);
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

        private void ChooseTargetToFight()
        {
            Ship[] ships = GameObject.FindObjectsOfType<Ship>();
            Ship closesShip = null;
            float closesDistanceSoFar = Mathf.Infinity;
            foreach(Ship s in ships)
            {
                if (s.Equals(this)) continue;
 
                if (closesShip == null) closesShip = s;
                else
                {
                    float distance = Vector3.Distance(ship.position, s.transform.position);
                    if (closesDistanceSoFar > distance)
                    {
                        closesDistanceSoFar = distance;
                        closesShip = s;
                    }
                }
            }

            if (closesShip != null) EnterBattleMode(closesShip.transform);
        }
    }
}
