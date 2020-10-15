using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace saltos
{
    public abstract class DragonfligGameMode : GameMode
    {
        [SerializeField] protected DragoflyChasingGameManager _gameManager;
    }
}