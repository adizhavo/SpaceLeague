using SpaceLeague.CameraShake;

namespace SpaceLeague.Ship.Weapon
{
    public class PlayerShipMainCannon : ShipMainCannon
    {
        protected override void Fire()
        {
            base.Fire();
            CameraShakeProvider.Instance.StartShake(ShakeType.Fire);
        }
    }
}