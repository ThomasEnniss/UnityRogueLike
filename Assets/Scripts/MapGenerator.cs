using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MapGenerator : MonoBehaviour {

	public GameObject player;

	public GameObject wall_prefab;
	public GameObject floor_prefab;
	public GameObject chest;
	public GameObject door_down_prefab;
	public GameObject door_side_prefab;

	public int map_width_x;
	public int map_length_z;
	public int number_of_rooms;

	public int min_room_width;
	public int max_room_width;

	public int min_room_length;
	public int max_room_length;

	public int max_chest_count;

	int[,] map_generation_grid;

	List<Room> current_room_list;
	RoomGraph current_map_graph;

	int actual_room_count;

	void Start () {
		//player_in_map = false;
		map_generation_grid = new int[map_width_x,map_length_z];
		current_room_list = new List<Room> ();
		actual_room_count = 0;
		GenerateMap ();
		CreateRoomGraph ();
		current_map_graph.GenerateEdgeConnections ();	
		current_map_graph.EMST();
		//CleanUpHallways ();
		current_map_graph.DisplayGraph ();
		CreatePaths ();
		DrawMap ();
		DrawDebugLines();
	}

	void GenerateMap(){
		CreateBlankMapArray ();
		CreateRooms ();
	}

	void CreateBlankMapArray(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				map_generation_grid[widthX,lengthZ] = 0;
			}
		}
	}

	void CreateRooms(){

		/*These change based on the current rooms position and the roomnode.*/
		int initial_x_min_constraint = 0;
		int initial_x_max_constraint = map_width_x - 1;
		int initial_z_min_constraint = 0;
		int initial_z_max_constraint = map_length_z - 1;

		int fail_count = 0;
		int MAX_FAIL_COUNT = 3;

		for (int i = 0; i < number_of_rooms; i++) {

			int room_width_x = (int)Random.Range (min_room_width,max_room_width);
			int room_length_z = (int)Random.Range (min_room_length,max_room_length);

			/*We need to make width odd so we can center things.*/
			/*Center width first*/
			if (room_width_x % 2 == 0) {

				if (room_width_x > min_room_width) {

					room_width_x--;

				} else {

					room_width_x++;

				}
			} 

			/*Center Length second*/
			if(room_length_z % 2 == 0){

				if(room_length_z > min_room_length){

					room_length_z--;

				}else{
				
					room_length_z++;

				}
			}				

			int top_x = (int)Random.Range (initial_x_min_constraint,initial_x_max_constraint - room_width_x);
			int top_z = (int)Random.Range (initial_z_min_constraint,initial_z_max_constraint - room_length_z);

			Room new_room = new Room (i,top_x,top_z,room_width_x,room_length_z);

			if (!RoomCollision (new_room)) {			

				new_room.CalculateCenter ();
				current_room_list.Add (new_room);
				fail_count = 0;
				actual_room_count++;
				PlotRoom (new_room);
				print ("Plotting room: " + i);

			}
			else
			{
				
				fail_count++;
				print ("Room Failed");

				if(fail_count<MAX_FAIL_COUNT){
					i--;
				}
			}
		}
	}

	void CreatePaths(){

		for(int i = 0;i<current_map_graph.edges.Count;i++){

			Edge current_edge = current_map_graph.edges[i];
			/*TE: Gotta work out where the doors will be.*/
			Room start_room = current_map_graph.room_graph [current_edge.start_room];
			Room destination_room = current_map_graph.room_graph [current_edge.destination_room];

			FindSuitableDoorLocation (start_room,destination_room);
			FindSuitableDoorLocation (destination_room,start_room);
		}
	}

	void FindSuitableDoorLocation(Room start, Room destination){


		float internal_angle = 90-Mathf.Rad2Deg*Mathf.Atan(start.width_x/start.length_z);

		float angle_to_destination = Vector3.Angle(Vector3.forward, (start.center - destination.center));

		Debug.Log (angle_to_destination);

		/*Top, then right, then left, then bottom last.*/
		if(angle_to_destination<=internal_angle){
			
			PlaceDoor(new Vector3 ((int)Random.Range(start.top_x+1,start.top_x+start.width_x-1),0,start.top_z),3);
		}else if(start.center.x <= destination.center.x && angle_to_destination > internal_angle && angle_to_destination < (180f-internal_angle)){
			
			PlaceDoor(new Vector3 (start.top_x+start.width_x,0,(int)Random.Range(start.top_z+1,start.top_z+start.length_z-1)),4);
		}else if(start.center.x > destination.center.x && angle_to_destination > internal_angle && angle_to_destination < (180f-internal_angle)){
			
			PlaceDoor(new Vector3 (start.top_x-1,0,(int)Random.Range(start.top_z+1,start.top_z+start.length_z-1)),4);
		}else{
			
			PlaceDoor(new Vector3 ((int)Random.Range(start.top_x+1,start.top_x+start.width_x-1),0,start.top_z+start.length_z),3);
		}
	}

	void BFSHallway(Vector3 current,Vector3 goal){














	}

	/*TE: 3 = top/down door. 4 = side door*/
	void PlaceDoor(Vector3 location, int door_type){

		map_generation_grid [(int)location.x, (int)location.z] = door_type;

	}


	bool RoomCollision(Room room){

		bool collision = false;

		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (map_generation_grid [widthX, lengthZ] != 0)
				{
					collision = true;
				}
			}
		}
		return collision;
	}

	/*Fills the room area on the map.*/
	void PlotRoom(Room room){

		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (lengthZ == room.top_z || lengthZ == room.top_z + room.length_z - 1 || widthX == room.top_x || widthX == room.top_x + room.width_x - 1)
				{					
					map_generation_grid [widthX, lengthZ] = 2;
				}
				else 
				{					
					map_generation_grid [widthX, lengthZ] = 1;		
				}
			}
		}
	}

	void CleanUpHallways(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				if(map_generation_grid[widthX,lengthZ] == 0){
					/*1 == FLOOR, 0 == SPACE, 2 == WALL*/
					Debug.Log ("Checking X: "+widthX+" Y: "+lengthZ);

					int surrounding_floors = ReturnSurroundingTileCount(widthX, lengthZ,1);

					if (surrounding_floors > 0) {
						map_generation_grid [widthX, lengthZ] = 2;
					}
				}
			}
		}
	}

	int ReturnSurroundingTileCount(int position_x,int position_z, int tile_type_to_check){

		int return_count = 0;

		/*Top Left*/
		if(position_x-1 >= 0 && position_z-1 >= 0){
			if (map_generation_grid [position_x - 1, position_z - 1] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Top*/
		if(position_z-1 >= 0){
			if (map_generation_grid[position_x,position_z-1] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Top Right*/
		if(position_x+1 < map_width_x-1 && position_z-1 >= 0){
			if (map_generation_grid[position_x+1,position_z-1] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Left*/
		if(position_x-1 >= 0){
			if (map_generation_grid[position_x-1,position_z] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Right*/
		if(position_x+1 < map_width_x-1){
			if (map_generation_grid[position_x+1,position_z] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Bottom Left*/
		if(position_x-1 >= 0 && position_z+1 < map_length_z-1){
			if (map_generation_grid[position_x-1,position_z+1] == tile_type_to_check) {
				return_count++;
			}
		}

		/*Bottom*/
		if(position_z+1 < map_length_z-1){
			if (map_generation_grid[position_x,position_z+1] == tile_type_to_check) {
				return_count++;
				Debug.Log ("Bottom X:" + position_x + " Y:" + position_z);
			}
		}

		/*Bottom Right*/
		if(position_x+1 < map_width_x-1 && position_z+1 < map_length_z-1){
			if (map_generation_grid[position_x+1,position_z+1] == tile_type_to_check) {
				return_count++;
			}
		}
		return return_count;
	}

	void CreateRoomGraph(){

		current_map_graph = new RoomGraph (actual_room_count);

		foreach(Room current_room in current_room_list){
			

			current_map_graph.AddRoom (current_room);

		}
	}

	void DrawMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{

				if(map_generation_grid [widthX, lengthZ]==1){
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);				
				}

				/*Instantiate a wall and floor tile.*/
				if (map_generation_grid [widthX, lengthZ]==2) {
					GameObject wall_tile = (GameObject)Instantiate (wall_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
				}

				if (map_generation_grid [widthX, lengthZ]==3) {
					GameObject wall_tile = (GameObject)Instantiate (door_down_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
				}

				if (map_generation_grid [widthX, lengthZ]==4) {
					GameObject wall_tile = (GameObject)Instantiate (door_side_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
				}
			}
		}
		print (Time.time);
	}

	/*Loop through graph and draw lines from the rooms to its connected room.*/
	void DrawDebugLines(){

		int number_of_nodes = current_map_graph.edges.Count;

		for (int i = 0; i < number_of_nodes; i++) {

			Edge current = current_map_graph.edges [i];

			Debug.DrawLine(current_map_graph.room_graph[current.start_room].center,current_map_graph.room_graph[current.destination_room].center,Color.green,120f,false);

		}
	}
}