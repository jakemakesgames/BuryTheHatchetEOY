using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Michael Corben
//Based on Tutorial:https://www.youtube.com/watch?v=rZAnnyensgs&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=3
//Created 25/07/2018
//Last edited 25/07/2018
public interface IDamagable {

    void TakeHit(int damage, RaycastHit hit);

}