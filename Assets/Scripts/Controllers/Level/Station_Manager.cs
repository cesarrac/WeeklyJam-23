using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station_Manager : MonoBehaviour {
	public static Station_Manager instance {get; protected set;}
	Station[] station_map;
	public Station current_station {get; protected set;}
	void Awake(){
		instance = this;
	}

	public void Initialize(){
		station_map = new Station[6];
		station_map[0] = new Station("Centrum");
		station_map[1] = new Station("Villa Rosa");
		station_map[2] = new Station("Tales");
		station_map[3] = new Station("Freedom Quarry");
		station_map[4] = new Station("Hierrus");
		station_map[5] = new Station("Yazabb");
		int totalNToSet = 0;
		foreach (Station station in station_map){
			// Set station neigbors total
			station.neighborTotal = Random.Range(2, 5);
			totalNToSet += station.neighborTotal;
		}
		// Set neighbors
		int neighborsSet = 0;
		while(neighborsSet < totalNToSet){
			for(int j = 0; j < station_map.Length; j++){
				if (station_map[j].neighbors.Count >= station_map[j].neighborTotal)
					continue;
				Station newNeighbor = null;
				int getNeighborAttempts = (station_map.Length - 1) - j;
				int attempts = 0;
				while(attempts < getNeighborAttempts){
					int selection = Random.Range(j + 1, station_map.Length);
					if (station_map[selection] == station_map[j])
						continue;
					if (station_map[selection].AddNeighbor(station_map[j]) == true){
							// Set it as the new neighbor
							neighborsSet += 1;
							newNeighbor = station_map[selection];
							attempts = getNeighborAttempts;
					}
					attempts++;
				}
				// If after while loop no neighbors were found, remove link from total
				if (newNeighbor == null){
					station_map[j].RemoveNeighborLink();
					totalNToSet -= 1;
					continue;
				}
				station_map[j].AddNeighbor(newNeighbor);
				neighborsSet += 1;
			}

		}

		// Set jump locations:
		// *** Make sure Centrum [0] has a neighbor that is on jump location of 1 
		station_map[0].SetJumpLocation(1);
		Station firstNeighbor = station_map[0].neighbors[Random.Range(0, station_map[0].neighbors.Count)].station;
		firstNeighbor.SetJumpLocation(1);
		for(int i = 1; i < station_map.Length; i++){
			if (station_map[i] == firstNeighbor)
				continue;
			station_map[i].SetJumpLocation(Random.Range(2, 6));
		}


	}
	public Station GetStation(int stationIndex){
		if (stationIndex < 0 || stationIndex >= station_map.Length)
			return null;
		return station_map[stationIndex];
	}
	public Station[] GetStationAtDistance(int jumpCapacity){
		List<Station> nearByStations = new List<Station>();
		foreach(Station station in station_map){
			if (station.jumpLocation <= jumpCapacity){
				nearByStations.Add(station);
			}
		}
		return nearByStations.ToArray();
	}
	public int GetStationIndex(string stationName){
		int index = 0;
		for(int i = 0; i < station_map.Length; i++){
			if (station_map[i].stationName == stationName){
				index = i;
				break;
			}
		}
		return index;
	}
	public int GetStationIndex(Station station){
		int index = 0;
		for(int i = 0; i < station_map.Length; i++){
			if (station_map[i] == station){
				index = i;
				break;
			}
		}
		return index;
	}
	public void OnEnterStation(Station station){
		current_station = station;
	}
	public void Undock(){
		Game_LevelManager.instance.ReplaceStateWith(StateType.StationExit);

	}
}
