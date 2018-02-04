using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Inventory_Controller : MonoBehaviour {
    
    string ownerID;
    public Inventory inventory {get; protected set;}

    public void Initialize(string uniqueID, int maxSpace){
        if (ownerID != null && ownerID != string.Empty && ownerID.Length > 0){
            if (ownerID != uniqueID){
                Debug.LogError("An Inventory controller with id " + ownerID + " is being initialized by " + uniqueID + " this would OVERRIDE the inventory!!");
                return;
            }
        }
        // First try to load the inventory
        Inventory savedInventory = Inventory_Manager.instance.LoadInventory(uniqueID);
        if (savedInventory != null){
            inventory = savedInventory;
        }
        else
            inventory = new Inventory(maxSpace);

        ownerID = uniqueID;
       // Debug.Log("Inventory id set to: " + ownerID);
        // TODO: Initialize the inventory's UI
        // We're probably going to have to grab it from the pool
        // and re-position / re-scale it to fit this inventory
    }

    private void OnDisable() {
        if (inventory != null){
            Inventory_Manager.instance.SaveInventory(ownerID, inventory);
            inventory = null;
            ownerID = string.Empty;
                
        }
    }
}