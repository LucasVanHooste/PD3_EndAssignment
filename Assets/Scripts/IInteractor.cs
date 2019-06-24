using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    void ObstacleInteraction(ObstacleScript obstacle);
    void GunInteraction(GunScript gun);
    void LadderInteraction(LadderScript ladder);
    void TurretInteraction(TurretScript turret);
}
