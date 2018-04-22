using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public GameObject player;

	public GameObject wall_prefab;
	public GameObject floor_prefab;
	public GameObject chest;

	public int room_space_width_x;
	public int room_space_length_z;
	public int number_of_rooms;

	public int min_room_width;
	public int max_room_width;

	public int min_room_length;
	public int max_room_length;

	public int max_chest_count;

	int[,] room_grid;

	int grid_x_constant;
	int grid_z_constant;

	List<Room> rooms_to_plot;



	void Start () {
		grid_x_constant = min_room_width+2;
		grid_z_constant = room_space_length_z+2;
		//player_in_map = false;
		room_grid = new int[room_space_width_x,room_space_length_z];
		rooms_to_plot = new List<Room> ();
		GenerateMap ();
		DrawMap ();
	}

	void GenerateMap(){
		FillMap ();
		CreateRooms ();
	}

	void FillMap(){

		for(int lengthZ = 0;lengthZ<room_space_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<room_space_width_x;widthX++)
			{
				room_grid[widthX,lengthZ] = 0;
			}
		}
	}

	void CreateRooms(){

		/*These change based on the current rooms position and the roomnode.*/
		int initial_x_min_constraint = 0;
		int initial_x_max_constraint = room_space_width_x - 1;
		int initial_z_min_constraint = 0;
		int initial_z_max_constraint = room_space_length_z - 1;			


		for (int i = 0; i < number_of_rooms; i++) {

			int grid_x = (int)Random.Range (initial_x_min_constraint,initial_x_max_constraint);
			int grid_z = (int)Random.Range (initial_z_min_constraint,initial_z_max_constraint);

			if (room_grid[grid_x,grid_z] == 0) {

				int width = (int)Random.Range (min_room_width,max_room_width);
				int length = (int)Random.Range (min_room_length,max_room_length);

				int top_x = (int)Random.Range (grid_x*grid_x_constant,grid_x*grid_x_constant+grid_x_constant-max_room_width-1);
				int top_z = (int)Random.Range (grid_z*grid_z_constant,grid_z*grid_z_constant+grid_z_constant-max_room_length-1);

				Room new_room = new Room (top_x,top_z,width,length);
				rooms_to_plot.Add (new_room);
				room_grid [grid_x, grid_z] = 1;

			} else {
				i++;
			}
		}
	}

	/*Fills the room area on the map.*/
	/*void PlotRoom(Room room){

		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (lengthZ == room.top_z || lengthZ == room.top_z + room.length_z - 1 || widthX == room.top_x || widthX == room.top_x + room.width_x - 1) {
					if (room_grid [widthX, lengthZ] != 0) {
						room_grid [widthX, lengthZ] = 2;
					}
				} else {
					
					room_grid [widthX, lengthZ] = 0;					
				}
			}
		}
	}*/

	void DrawMap(){

		foreach (Room new_room in rooms_to_plot) {

			for(int lengthZ = new_room.top_z;lengthZ<new_room.top_z+new_room.length_z;lengthZ++)
			{
				for(int widthX = new_room.top_x;widthX<new_room.top_x+new_room.width_x;widthX++)
				{
					/*Instantiate a wall and floor tile.*/
					if (lengthZ == new_room.top_z || lengthZ == new_room.top_z + new_room.length_z - 1 || widthX == new_room.top_x || widthX == new_room.top_x + new_room.width_x - 1) {
						GameObject wall_tile = (GameObject)Instantiate (wall_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
						GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
					} else {
						GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);				
					}
				}
			}
		}		
	}
}
