using UnityEngine;

public class BuildableController : MonoBehaviour {
    
    public void Init(Buildable buildable){
        Debug.Log("Buildable initialized with " + buildable.name);
    }
}