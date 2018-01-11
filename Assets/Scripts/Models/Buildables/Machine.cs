using UnityEngine;

public enum MachineCondition {Hopeless, Down, Decayed, OK, Good, Pristine}
public class Machine : Buildable {
    
    
/* 	
	public float efficiencyRate = 50;
	public float startDiffOffset = 0.1f; */
    public int tileWidth = 1;
	public int tileHeight = 2;
	public ShipSystemType systemControlled = ShipSystemType.None;
	public MiniGameDifficulty repairDifficulty;
    public MachineCondition machineCondition {get; protected set;}
    protected Machine(MachinePrototype b) : base(b.name, b.sprite, b.animatorController, BuildableType.Machine, b.stats) {
        this.systemControlled = b.systemControlled;
        this.repairDifficulty = b.repairDifficulty;
        this.tileWidth = b.tileWidth;
        this.tileHeight = b.tileHeight;
        this.machineCondition = b.machineCondition;
    }
    protected Machine(ProducerPrototype prodProto) : base(prodProto.name, prodProto.sprite, prodProto.animatorController, BuildableType.Producer, prodProto.stats){
        this.systemControlled = ShipSystemType.None;
        this.repairDifficulty = prodProto.repairDifficulty;
        this.tileHeight = prodProto.tileHeight;
        this.tileWidth = prodProto.tileWidth;
        this.machineCondition = prodProto.machineCondition;
    }
    public static Machine CreateInstance(MachinePrototype prototype){
        return new Machine(prototype);
    }

    public void RepairCondition(){
        // called if mini game was succesful
        // RETURN if already at MAX condition
        if ((int)machineCondition >= System.Enum.GetValues(typeof(MachineCondition)).Length)
            return;
        int curCondition = (int)machineCondition;
        
        machineCondition = (MachineCondition) curCondition + 1;
    }
    public void DecayCondition(){
        int curCondition = (int)machineCondition;
        if (curCondition <= 0){
            return;
        }
        machineCondition = (MachineCondition) curCondition - 1;
    }
}