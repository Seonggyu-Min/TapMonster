using System;
using UnityEngine;

public class PlayerPresenter : IDisposable
{
    private PlayerView _playerView;
    private SkillManager _skillManager;
    private ManualInputPresenter _inputManager;
    

    public PlayerPresenter(
        PlayerView playerView,
        SkillManager skillManager,
        ManualInputPresenter inputManager
        )
    {
        _playerView = playerView;
        _skillManager = skillManager;
        _inputManager = inputManager;
    }


    public void Initialize()
    {
        //_inputManager.OnManualAttack += HandleManualAttack;
        //_skillManager.OnSkillUsed += HandleSkillUsed;

    }



    public void Dispose()
    {
        
    }
}
