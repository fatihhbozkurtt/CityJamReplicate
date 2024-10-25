using MoreMountains.NiceVibrations;


namespace EssentialManagers.Scripts
{
    public class HapticManager : MonoSingleton<HapticManager>
    {
        public void TriggerHaptic(HapticTypes hapticType)
        {
            MMVibrationManager.Haptic(hapticType);
        }
    }
}