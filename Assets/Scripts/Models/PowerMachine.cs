using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power", menuName = "Machines/New Power")]
public class PowerMachine : Machine_Data
{
    public override void Init(Machine_Controller controller)
    {
        machine_Controller = controller;
        machine_Controller.InitData(machineName, tileWidth, tileHeight, systemControlled, efficiencyRate);
    }
}
