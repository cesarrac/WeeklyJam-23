using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Machine_Controller : MonoBehaviour {
    public Machine machine {get; protected set;}
    Vector3Int worldPosition;
    public Tile_Data baseTile;
    ShipManager shipManager;
    public List<Tile_Data> neighborTiles {get; protected set;}
    public BoxCollider2D collidable;
    public SpriteRenderer shadowRenderer;
    public Animator animator {get; protected set;}
    Action onRepairDoneCB;
    public GameObject damageFX;
    public Item machineItem {get; protected set;}
    SpriteRenderer mainSpriteR;
    void OnEnable(){
        animator = GetComponent<Animator>();
        mainSpriteR = GetComponent<SpriteRenderer>();
    }

    public void InitMachine(Item _machineItem, Machine _machine, Tile_Data tile, ShipManager ship){
        if (tile.AddMachine(this) == false)
            return;
        // Store the machine as Item that can be placed into an inventory
        machineItem = _machineItem;
        machine = _machine;
        gameObject.name = machine.name;
        baseTile = tile;
        worldPosition = tile.worldPos;
        shipManager = ship;
        neighborTiles = new List<Tile_Data>();
        if (machine.tileWidth > 1 || machine.tileHeight > 1){
            if (baseTile == null){
                return;
            }
            for(int x = 0; x < machine.tileWidth; x++){
                    for(int y = 0; y < machine.tileHeight; y++){
                        Tile_Data neighbor = TileManager.instance.GetTile(baseTile.X + x, baseTile.Y + y);
                        if (neighbor != null && neighbor != baseTile){
                            if (neighbor.AddMachine(this) == true){
                                neighborTiles.Add(neighbor);
                                //Debug.Log("machine added to neighbor at " + neighbor.worldPos);
                            }
                        }
                    }
            }
            InitVisuals();
        
        } 
          
    }
    public void ReInit(Item _machineItem, Machine _machine, ShipManager ship, Tile_Data tile, List<Tile_Data> neighbors){
         // Store the machine as Item that can be placed into an inventory
        machineItem = _machineItem;
        machine = _machine;
        gameObject.name = machine.name;
        baseTile = tile;
        worldPosition = tile.worldPos;
        shipManager = ship;
        neighborTiles = neighbors;
        InitVisuals();
    }
    void InitVisuals(){
        mainSpriteR.sprite = machine.sprite;
        animator.runtimeAnimatorController = machine.animatorController;

        collidable.offset = new Vector2(machine.tileWidth > 1 ? 1 : 0.5f, 0.5f);
        collidable.size  = new Vector2(machine.tileWidth, 1);
        /* interactable.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, tileHeight > 1 ? 1 : 0.5f);
        interactable.size  = new Vector2(tileWidth, tileHeight); */
        shadowRenderer.size = new Vector2(machine.tileWidth, 1);

        damageFX.GetComponent<SpriteRenderer>().size = new Vector2(machine.tileWidth,2);
    }
    public bool CanUse(){
        if ((int)machine.machineCondition < 2){
            return false;
        }
        return true;
    }
    public void UseMachine(){
        
        // Play animation of machine being used
        AnimateOn();

        float roll = UnityEngine.Random.Range (1, 100);
        if (roll <= machine.GetStat(StatType.Efficiency).GetValue()){
            // Machine does NOT decay
            return;
        }

        DecayCondition();
    
    }
    public virtual void UserUseMachine(){
        UI_Manager.instance.ShowMachineUI(machine.systemControlled);
        AnimateOn();
    }
    public virtual void Interact(GameObject user){
        if (shipManager.SystemInteract(machine.systemControlled, user) == true){
            AnimateOn();
        }
    }
    public void AnimateOn(){
        // animate machine to show it being interfaced
        animator.SetTrigger("on");
    }
    public void AnimateStayOn(){
        // animate machine to show it being interfaced
        animator.SetTrigger("stay");
    }
    public void AnimateOff(){
        // animate machine to show it being interfaced
        animator.SetTrigger("off");
    }
    public void TryRepair(Action onDoneCB){
        if (onDoneCB != null)
            onRepairDoneCB += onDoneCB;
        // Start mini game ui
        MiniGameManager.instance.StartRepairGame(this);
        AnimateStayOn();
    }
    public void CancelRepair(){
        if (onRepairDoneCB != null)
            onRepairDoneCB();
        MiniGameManager.instance.CancelRepairGame();
        AnimateOff();
        onRepairDoneCB = null;
    }
    public void RepairSuccess(){
        if (onRepairDoneCB != null)
            onRepairDoneCB();
      
        RepairCondition();
        AnimateOff();
        onRepairDoneCB = null;
    }
    public void RepairFail(){
        if (onRepairDoneCB != null)
            onRepairDoneCB();
        
        AnimateOff();
        onRepairDoneCB = null;
    }
    void RepairCondition(){
        // called if mini game was succesful
        // RETURN if already at MAX condition
        machine.RepairCondition();

        if (damageFX.activeSelf == true){
            damageFX.SetActive(false);
        }
        Notification_Manager.instance.AddNotification(machine.name + " repaired to " + machine.machineCondition);
        Debug.Log("MACHINE REPAIRED! " + machine.machineCondition);
    }
    public void DecayCondition(){
        machine.DecayCondition();

        Notification_Manager.instance.AddNotification(machine.name + " decayed to " + machine.machineCondition);
        // Activate damage FX animation
        if (machine.machineCondition < MachineCondition.Decayed){
            if (damageFX.activeSelf == false){
                damageFX.SetActive(true);
            }
        }
    }

    public virtual void RemoveFromTile(){
         if (baseTile != null){
			if (baseTile.RemoveMachine() == true){
				Debug.Log(machine.name + " removed from tile");
			}
		}
        if (neighborTiles != null && neighborTiles.Count > 0){
            foreach(Tile_Data tile in neighborTiles){
                tile.RemoveMachine();
            }
        }
    }
    public virtual void Pool(){
        if (animator != null)
            animator.runtimeAnimatorController = null;
		// Pool this machine's gameobject
        this.gameObject.name = "Machine";

        Buildable_Manager.instance.PoolBuildable(machine);
    }
    public void RemoveAndPool(){
       
        RemoveFromTile();
        Pool();
    }
}
