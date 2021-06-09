using System.ComponentModel;
using UnityEngine;
using Zenject;

public class DefaultInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager SFXManager;
    [SerializeField] private AudioManager BGMManager;
    public override void InstallBindings()
    {
        InstallGameManager();
        InstallAudio();
    }

    private void InstallAudio()
    {
        if (SFXManager != null)
            Container.Bind<AudioManager>().WithId("SFXManager").FromComponentInNewPrefab(SFXManager).AsSingle();
        if (BGMManager != null)
            Container.Bind<AudioManager>().WithId("BGMManager").FromComponentInNewPrefab(BGMManager).AsSingle();
    }

    private void InstallGameManager()
    {
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<PlaySessionDataHandler>().AsSingle();
        Container.Bind<UserDataManager>().FromInstance((UserDataManager)FindObjectOfType(typeof(UserDataManager))).AsSingle();
    }
}