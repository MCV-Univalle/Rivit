using System.ComponentModel;
using UnityEngine;
using Zenject;

public class DefaultInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager SFXManager;
    [SerializeField] private AudioManager BGMManager;
    //[SerializeField] private UIManager uiManagerPrefab;
    public override void InstallBindings()
    {
        //Container.Bind<GameManager>().FromComponentInNewPrefab(gameManagerPrefab).AsSingle();
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        Container.Bind<RankingManager>().AsSingle().WithArguments(gameManager.Name);
        Container.Bind<PauseManager>().AsSingle();

        if(SFXManager != null)
        Container.Bind<AudioManager>().WithId("SFXManager").FromComponentInNewPrefab(SFXManager).AsSingle();
        if(BGMManager != null)
        Container.Bind<AudioManager>().WithId("BGMManager").FromComponentInNewPrefab(BGMManager).AsSingle();
    }
}