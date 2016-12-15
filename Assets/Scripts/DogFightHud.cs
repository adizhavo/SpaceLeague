using UnityEngine;
using UnityEngine.UI;
using SpaceLeague.Ship;
using SpaceLeague.Ship.Player;

namespace SpaceLeague.UI
{
    public class DogFightHud : MonoBehaviour 
    {
        public Image dogFightFiller;
        public Button dogFightButton;

        private PlayerShip playerShip;

        private void Start()
        {
            dogFightButton.onClick.AddListener(TriggerDogFight);
        }

        private void Update()
        {
            if (playerShip == null) 
            {
                ReachPlayerShip();
                return;
            }

            dogFightFiller.fillAmount = playerShip.currentDogFightFiller / playerShip.maxDogFightFiller;
            dogFightButton.interactable = playerShip.IsReadyForDogFight || playerShip.currentFlyMode.Equals(ShipFlyMode.DogFight);
        }

        private void ReachPlayerShip()
        {
            playerShip = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShip>();
        }

        private void TriggerDogFight()
        {
            playerShip.EnterDogFight();
        }
    }
}