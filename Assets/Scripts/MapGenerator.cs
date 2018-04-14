using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public GameObject parent_map;
	public GameObject wall_prefab;
	public GameObject floor_prefab;
	public int map_width_x;
	public int map_length_z;
	public int number_of_rooms;

	public int min_room_width;
	public int max_room_width;

	public int min_room_length;
	public int max_room_length;

	int[,] map_structure;

	// Update is called once per frame
	void Start () {
		map_structure = new int[map_width_x,map_length_z];

		GenerateMap ();
		DrawMap ();
	}

	void GenerateMap(){
		FillMap ();
		CreateRooms ();
	}

	void FillMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				map_structure[widthX,lengthZ] = 1;
			}
		}
	}

	void CreateRooms(){



		/*These change based on the current rooms position and the roomnode.*/
		int initial_x_min_constraint = 1;
		int initial_x_max_constraint = map_width_x- 1- max_room_width;
		int initial_z_min_constraint = 1;
		int initial_z_max_constraint = map_length_z - 1 - max_room_length;			


		for (int i = 0; i < number_of_rooms; i++) {

			int top_x = (int)Random.Range (initial_x_min_constraint,initial_x_max_constraint);
			int top_z = (int)Random.Range (initial_z_min_constraint,initial_z_max_constraint);
			int width = (int)Random.Range (min_room_width,max_room_width);
			int length = (int)Random.Range (min_room_length,max_room_length);

			Room new_room = new Room (top_x,top_z,width,length);

			PlotRoom (new_room);

		}
	}





	/*Fills the room area on the map. Marks it as 0*/
	void PlotRoom(Room room){
		
		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (lengthZ == room.top_z || lengthZ == room.top_z + room.length_z - 1 || widthX == room.top_x || widthX == room.top_x + room.width_x - 1) {
					if (map_structure [widthX, lengthZ] != 0) {
						map_structure [widthX, lengthZ] = 2;
					}
				} else {
					if (map_structure [widthX, lengthZ] != 2) {
						map_structure [widthX, lengthZ] = 0;
					}
				}
			}
		}
	}

	void DrawMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				
				if(map_structure[widthX,lengthZ] == 0){
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
					floor_tile.transform.SetParent (parent_map.transform);
				}

				if(map_structure[widthX,lengthZ] == 2){
					GameObject wall_tile = (GameObject)Instantiate (wall_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					wall_tile.transform.SetParent (parent_map.transform);
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
					floor_tile.transform.SetParent (parent_map.transform);
				}


			}
		}
	}
}
