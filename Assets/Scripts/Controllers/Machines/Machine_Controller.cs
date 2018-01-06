using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MachineCondition {Hopeless, Down, Decayed, OK, Good, Pristine}
public class Machine_Controller : MonoBehaviour {
    public MachineCondition machineCondition {get; protected set;}
    public string machineName = "Machine";
    int tileWidth = 1;
    int tileHeight = 2;
    public ShipSystemType shipSystemsControlled {get; protected set;}
    Vector3Int worldPosition;
    public Tile_Data baseTile;
    ShipManager shipManager;
    List<Tile_Data> neighborTiles;
    float efficiencyRate = 1;
    public MiniGameDifficulty repairDifficulty {get; protected set;}
    public BoxCollider2D collidable;
    public SpriteRenderer shadowRenderer;
    Animator animator;
    Action onRepairDoneCB;
    public GameObject damageFX;
    public Item machineItem {get; protected set;}
    SpriteRenderer mainSpriteR;
    void OnEnable(){
        animator = GetComponent<Animator>();
        mainSpriteR = GetComponent<SpriteRenderer>();
    }
    public void InitData(string _name, Sprite machineSprite, RuntimeAnimatorController animatorController, int _tileWidth, int _tileHeight,ShipSystemType _systemsControlled, float _efficiency, MiniGameDifficulty _repairDifficulty){
        machineName = _name;
        tileWidth = _tileWidth;
        tileHeight = _tileHeight;
        shipSystemsControlled = _systemsControlled;
        efficiencyRate = _efficiency;   
        repairDifficulty = _repairDifficulty;
        mainSpriteR.sprite = machineSprite;
        animator.runtimeAnimatorController = animatorController;
        Debug.Log("Data initialized " + _name + " with sprite " + machineSprite.name);
    }

    public void InitMachine(Item _machineItem, Tile_Data tile, ShipManager ship){
        if (tile.AddMachine(this) == false)
            return;
        // Store the machine as Item that can be placed into an inventory
        machineItem = _machineItem;
        machineCondition = MachineCondition.OK;
        gameObject.name = machineName;
        baseTile = tile;
        worldPosition = tile.worldPos;
        shipManager = ship;
        neighborTiles = new List<Tile_Data>();
        if (tileWidth > 1 || tileHeight > 1){
            if (baseTile == null){
                return;
            }
            for(int x = 0; x < tileWidth; x++){
                    for(int y = 0; y < tileHeight; y++){
                        Tile_Data neighbor = TileManager.instance.GetTile(baseTile.X + x, baseTile.Y + y);
                        if (neighbor != null && neighbor != baseTile){
                            if (neighbor.AddMachine(this) == true){
                                neighborTiles.Add(neighbor);
                                //Debug.Log("machine added to neighbor at " + neighbor.worldPos);
                            }
                        }
                    }
            }

        
        } 
           collidable.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, 0.5f);
           collidable.size  = new Vector2(tileWidth, 1);
           /* interactable.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, tileHeight > 1 ? 1 : 0.5f);
           interactable.size  = new Vector2(tileWidth, tileHeight); */
           shadowRenderer.size = new Vector2(tileWidth, 1);

           damageFX.GetComponent<SpriteRenderer>().size = new Vector2(tileWidth, 2);
    }
    public bool CanUse(){
        if ((int)machineCondition < 2){
            return false;
        }
        return true;
    }
    public void UseMachine(){
        
        // Play animation of machine being used
        animator.SetTrigger("on");

        float roll = UnityEngine.Random.Range (1, 100);
        if (roll <= efficiencyRate){
            // Machine does NOT decay
            return;
        }

        DecayCondition();
    
    }
    public void DisplayMachineUI(){
        UI_Manager.instance.ShowMachineUI(shipSystemsControlled);
        AnimateOn();
    }
    public bool Interact(GameObject user){
        if (shipManager.SystemInteract(shipSystemsControlled, user) == true){
            AnimateOn();
            return true;
        }
        return false;
    }
    public void AnimateOn(){
        // animate machine to show it being interfaced
        animator.SetTrigger("on");
    }
    public void TryRepair(Action onDoneCB){
        if (onDoneCB != null)
            onRepairDoneCB += onDoneCB;
        // Start mini game ui
        MiniGameManager.instance.StartRepairGame(this);
        animator.SetTrigger("stay");
    }
    public void RepairSuccess(){
        if (onRepairDoneCB != null)
            onRepairDoneCB();
      
        RepairCondition();
        animator.SetTrigger("off");
        onRepairDoneCB = null;
    }
    public void RepairFail(){
        if (onRepairDoneCB != null)
            onRepairDoneCB();
        
        animator.SetTrigger("off");
        onRepairDoneCB = null;
    }
    void RepairCondition(){
        // called if mini game was succesful
        // RETURN if already at MAX condition
        if ((int)machineCondition >= System.Enum.GetValues(typeof(MachineCondition)).Length)
            return;
        int curCondition = (int)machineCondition;
        
        machineCondition = (MachineCondition) curCondition + 1;
        if (damageFX.activeSelf == true){
            damageFX.SetActive(false);
        }
        Notification_Manager.instance.AddNotification(shipSystemsControlled + " machine repaired to " + machineCondition);
        Debug.Log("MACHINE REPAIRED! " + machineCondition);
    }
    public void DecayCondition(){
        int curCondition = (int)machineCondition;
        if (curCondition <= 0){
            return;
        }
        machineCondition = (MachineCondition) curCondition - 1;
        Notification_Manager.instance.AddNotification(shipSystemsControlled + " machine's condition decayed to " + machineCondition);
        // Activate damage FX animation
        if (machineCondition < MachineCondition.Decayed){
            if (damageFX.activeSelf == false){
                damageFX.SetActive(true);
            }
        }
   
    }

    public void RemoveMachine(){
        if (baseTile != null){
			if (baseTile.RemoveMachine() == true){
				Debug.Log(machineName + " removed from tile");
			}
		}
        if (neighborTiles != null && neighborTiles.Count > 0){
            foreach(Tile_Data tile in neighborTiles){
                tile.RemoveMachine();
            }
        }
        if (animator != null)
            animator.runtimeAnimatorController = null;
		// Pool this machine's gameobject
        this.gameObject.name = "Machine";
        Item_Manager.instance.PoolItem(machineItem);
    }
}
