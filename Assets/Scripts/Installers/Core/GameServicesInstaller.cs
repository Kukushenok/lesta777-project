using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Installers
{
    public class GameServicesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TMPEntryPoint>();
        }
    }
    // TODO: do the actual game instead of this atrocity (loading scenes, etc.)
    public class TMPEntryPoint : ITickable
    {
        private readonly InputAction _act;
        [Inject]
        public TMPEntryPoint(InputActionAsset acts)
        {
            _act = acts.FindAction("Move");
        }
        public void Tick()
        {
            Vector2 x = _act.ReadValue<Vector2>();
            if (x.magnitude > 0)
            {
                Debug.Log($"Moving!! {x}");
            }
        }
    }
}
