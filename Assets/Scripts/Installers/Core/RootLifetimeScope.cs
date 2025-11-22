using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Installers
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [Header("Все файлы конфигов регистрируем здесь")]
        [SerializeField] private InputActionAsset _inputActions;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_inputActions);
            // if NORMAL
            new GameServicesInstaller().Install(builder);
            // if DEBUG
            // ... install something else
        }
    }
}
