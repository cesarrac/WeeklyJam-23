using UnityEngine;
using System;

public class ID_Generator{
    public static ID_Generator instance {get; protected set;}
    public ID_Generator(){
        instance = this;
    }

    public string GetMachineID(Machine_Controller machine_Controller){
        if (machine_Controller == null)
            return string.Empty;
        if (machine_Controller.machine == null)
            return string.Empty;
        if (machine_Controller.baseTile == null)
            return string.Empty;
        string id = machine_Controller.machine.name;
        id += machine_Controller.baseTile.X + "_" + machine_Controller.baseTile.Y;
        return id;
    }
}