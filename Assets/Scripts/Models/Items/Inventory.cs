using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory{
   
    public InventoryItem[] inventory_items;
    public int maxSpaces {get; protected set;}
    int spacesFilled;
    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChanged; 

    public Inventory(int maxSpace){
        maxSpaces = maxSpace;
        spacesFilled = 0;
        inventory_items  = new InventoryItem[maxSpaces];
        for(int i= 0; i <inventory_items.Length; i++){
            inventory_items[i] = new InventoryItem(null);
        }
    }
    public bool IsFull(){
        if (spacesFilled >= maxSpaces){
           return true;
        }
        return false;
    }
    public bool IsEmpty(){
        if (spacesFilled == 0)
            return true;
        return false;
    }
    public bool HasSpaceFor(int count){
        if ((maxSpaces - spacesFilled) < count)
            return false;
        return true;
    }
    public bool AddItem(Item item, int count = 1){
        if (count == 0){
            return false;
        }
        if (item.stackCount > 1){
            int index = ContainsItem(item.name);
            if (index >= 0){
                inventory_items[index].count += count;
                if (onInventoryChanged != null)
                    onInventoryChanged();
                return true;
            }
        }
        if (IsFull()){
           return false;
        }
        // Find first empty space
        int emptyIndex = -1;
        for(int i= 0; i <inventory_items.Length; i++){
           if (inventory_items[i].item == null){
               emptyIndex = i;
               break;
           }
        }
        if (emptyIndex < 0){
            return false; // no empty space found
        }
        inventory_items[emptyIndex].item = item;
        inventory_items[emptyIndex].count = count;

        spacesFilled ++;

        if (onInventoryChanged != null)
            onInventoryChanged();
        return true;
    }
    public int ContainsItem(string itemName){
        for(int i = 0; i < inventory_items.Length; i++){
            if (inventory_items[i].item != null){
                if (inventory_items[i].item.name == itemName && inventory_items[i].count > 0){
                    return i;
                }
            }
        }
        return -1;
    }
    public bool ContainsItem(string itemName, int count = 1){
        if (inventory_items.Length == 0)
            return false;
        Debug.Log("Checking if inventory contains " + count + " " + itemName);
         for(int i = 0; i < inventory_items.Length; i++){
            if (inventory_items[i].item == null)
                continue;
            if (inventory_items[i].item.name == itemName && inventory_items[i].count >= count){
                    return true;
            }
         }
        return false;
    }
    public bool RemoveItem(string itemName){
        int index = ContainsItem(itemName);
        if (index < 0){
            return false;
        }
        inventory_items[index].count -= 1;
        if (inventory_items[index].count <= 0){
            // make item null
            inventory_items[index].item = null;
            spacesFilled--;
        }

        if (onInventoryChanged != null)
            onInventoryChanged();
        return true;
    }
    public bool RemoveItem(int inventoryIndex){
        if (inventory_items.Length <= inventoryIndex){
            return false;
        }
        if (inventory_items[inventoryIndex].item == null)
            return false;
        if (inventory_items[inventoryIndex].count <= 0)
            return false;
        
        inventory_items[inventoryIndex].count -= 1;
        if (inventory_items[inventoryIndex].count <= 0){
            // make item null
            inventory_items[inventoryIndex].item = null;
            spacesFilled--;
        }
        
        if (onInventoryChanged != null)
            onInventoryChanged();
        return true;
    }
    public bool RemoveItem(string itemName, int count = 1){
        if (count == 0){
            return true;
        }
        

         for(int i = 0; i < inventory_items.Length; i++){
            if (inventory_items[i].item != null){
                if (inventory_items[i].item.name == itemName && inventory_items[i].count >= count){
                    inventory_items[i].count -= count;
                    if(inventory_items[i].count <= 0){
                        inventory_items[i].item = null;
                        spacesFilled--;
                    }
                    if (onInventoryChanged != null)
                        onInventoryChanged();
                    return true;
                }
            }
         }
         return false;
    }

}

public struct InventoryItem{
    public int count;
    public Item item;
    public InventoryItem(Item _item){
        item = _item;
        count = 0;
    }
}