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
		PlacePlayer ();
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

			//Vector3 start = FindSuitableDoorLocation (start_room,destination_room);
			//Vector3 goal = FindSuitableDoorLocation (destination_room,start_room);

			CreateLazyPath (start_room,destination_room);
		}
		AddDoors ();
		CleanUpPaths ();
		CleanUpHallways ();
	}

	void CreateLazyPath(Room start, Room goal){

		int current_x = (int)start.center.x;
		int current_z = (int)start.center.z;

		int x_increment;
		int z_increment;

		if (current_x > goal.center.x) {
			x_increment = -1;
		} else {
			x_increment = 1;
		}

		if (current_z > goal.center.z) {
			z_increment = -1;
		} else {
			z_increment = 1;
		}

		while(current_x!=goal.center.x){
			
			current_x+=x_increment;

			if(map_generation_grid [current_x, (int)start.center.z] == 2){
				map_generation_grid [current_x, (int)start.center.z] = 1;	
			}else{
				if(map_generation_grid [current_x, (int)start.center.z]==0){
					map_generation_grid [current_x, (int)start.center.z] = 5;
				}
			}
		}

		while(current_z!=goal.center.z){
			
			current_z+=z_increment;

			if(map_generation_grid [current_x, current_z] == 2){
				map_generation_grid [current_x, current_z] = 1;	
			}else{
				if(map_generation_grid [current_x, current_z]==0){
					map_generation_grid [current_x, current_z] = 5;
				}
			}
		}
	}


/*	Vector3 FindSuitableDoorLocation(Room start, Room destination){


		float internal_angle = 90-Mathf.Rad2Deg*Mathf.Atan(start.width_x/start.length_z);

		float angle_to_destination = Vector3.Angle(Vector3.forward, (start.center - destination.center));

		Debug.Log (angle_to_destination);

		
		if(angle_to_destination<=internal_angle){
			
			return PlaceDoor(new Vector3 ((int)Random.Range(start.top_x+1,start.top_x+start.width_x-1),0,start.top_z),3);
		}else if(start.center.x <= destination.center.x && angle_to_destination > internal_angle && angle_to_destination < (180f-internal_angle)){
			
			return PlaceDoor(new Vector3 (start.top_x+start.width_x-1,0,(int)Random.Range(start.top_z+1,start.top_z+start.length_z-1)),4);
		}else if(start.center.x > destination.center.x && angle_to_destination > internal_angle && angle_to_destination < (180f-internal_angle)){
			
			return PlaceDoor(new Vector3 (start.top_x,0,(int)Random.Range(start.top_z+1,start.top_z+start.length_z-1)),4);
		}else{
			
			return PlaceDoor(new Vector3 ((int)Random.Range(start.top_x+1,start.top_x+start.width_x-1),0,start.top_z+start.length_z-1),3);
		}
	}


	Vector3 PlaceDoor(Vector3 location, int door_type){

		map_generation_grid [(int)location.x, (int)location.z] = door_type;

		return location;
	}

	bool BFSHallway(Vector3 current, Vector3 goal){

		if (current.x != goal.x && current.z != goal.z) {
			
			List<Pathnode> possible_node = new List<Pathnode> ();


			if (current.z - 1 > 0) {
				if (map_generation_grid [(int)current.x, (int)current.z - 1] != 2) {
					if(!visited[(int)current.x,(int)current.z-1]){

						visited[(int)current.x,(int)current.z-1] = true;
						Vector3 up = new Vector3 (current.x, current.y, current.z - 1);
						Pathnode new_up_node = new Pathnode (up,Vector3.Distance(current,up));
						possible_node.Add(new_up_node);
					}
				}			
			}

	
			if (current.x + 1 < map_width_x - 2) {
				if (map_generation_grid [(int)current.x + 1, (int)current.z] != 2) {
					if(!visited[(int)current.x+1,(int)current.z]){

						visited[(int)current.x+1,(int)current.z] = true;
						Vector3 right = new Vector3 (current.x + 1, current.y, current.z);
						Pathnode new_right_node = new Pathnode (right,Vector3.Distance(current,right));
						possible_node.Add(new_right_node);
					}
				}			
			}


			if (current.x - 1 > 0) {
				if (map_generation_grid [(int)current.x - 1, (int)current.z] != 2) {
					if(!visited[(int)current.x-1,(int)current.z]){

						visited[(int)current.x-1,(int)current.z] = true;
						Vector3 left = new Vector3 (current.x - 1, current.y, current.z);
						Pathnode new_left_node = new Pathnode (left,Vector3.Distance(current,left));
						possible_node.Add(new_left_node);
					}
				}			
			}

	
			if (current.z + 1 < map_length_z) {
				if (map_generation_grid [(int)current.x, (int)current.z + 1] != 2) {
					if(!visited[(int)current.x,(int)current.z+1]){
						
						visited[(int)current.x,(int)current.z+1] = true;
						Vector3 down = new Vector3 (current.x, current.y, current.z + 1);
						Pathnode new_down_node = new Pathnode (down,Vector3.Distance(current,down));
						possible_node.Add(new_down_node);
					}
				}
			}

			int number_of_nodes = possible_node.Count;

		
			for (int k = number_of_nodes - 1; k >= 0; k--) {
				for(int l = 1; l<k;l++){
					if(possible_node[l].distance_to_target < possible_node[l-1].distance_to_target){
						Pathnode temp = possible_node[l];
						possible_node [l] = possible_node [l - 1];
						possible_node [l - 1] = temp;
					}
				}
			}

			if (number_of_nodes > 0) {
				for (int i = 0; i < number_of_nodes; i++) {					
					if (BFSHallway (possible_node [i].location, goal)) {
						if(map_generation_grid [(int)current.x, (int)current.z] != 3 && map_generation_grid [(int)current.x, (int)current.z] != 4){
							map_generation_grid [(int)current.x, (int)current.z] = 1;
						}
						return true;
					}
				}
				return false;
			} else {
				return false;
			}
		}
		else
		{
			return true;
		}
	}*/




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

	void AddDoors(){
		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				if(map_generation_grid[widthX,lengthZ] == 5){
					if(map_generation_grid[widthX-1,lengthZ]==1){
						map_generation_grid [widthX-1, lengthZ] = 4;
					}

					if(map_generation_grid[widthX+1,lengthZ]==1){
						map_generation_grid [widthX+1, lengthZ] = 4;
					}

					if(map_generation_grid[widthX,lengthZ-1]==1){
						map_generation_grid [widthX, lengthZ-1] = 3;
					}

					if(map_generation_grid[widthX,lengthZ+1]==1){
						map_generation_grid [widthX, lengthZ+1] = 3;
					}
				}
			}
		}

	}

	void CleanUpPaths(){
		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				if(map_generation_grid[widthX,lengthZ] == 5){
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

	void PlacePlayer(){

		Room start_room = current_map_graph.room_graph [0];

		Instantiate(player,new Vector3(start_room.center.x,1,start_room.center.z),Quaternion.identity);
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

			Debug.DrawLine(current_map_graph.room_graph[current.start_room].center,current_map_graph.room_graph[current.destination_room].center,Color.green,360f,false);

		}
	}
}