using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room{

	public int room_id;
	public int top_x;
	/*Since we are in 3D space y is the vertical and the plain is on the x z. I am treating Z as the y axis like I would in console rogue.*/
	public int top_z;

	public int width_x;
	public int length_z; /*would be height if using y in 2D space*/

	public Vector3 center;

	public Room(int new_id,int top_x_coordinate, int top_z_coordinate,int width,int length){

		top_x = top_x_coordinate;
		top_z = top_z_coordinate;

		width_x = width;
		length_z = length;

		CalculateCenter ();
	}

	public void CalculateCenter(){
		center = new Vector3 (top_x+((width_x-1)/2),0,top_z+((length_z-1)/2));
	}

	/*public void AddChest(){

		int item_x = Random.Range (top_x + 1,top_x + width_x - 1);
		int item_z = Random.Range (top_z + 1,top_z + length_z - 1);

		Vector3 new_item_position = new Vector3 (item_x,0.5f,item_z);
		contents.Add (new_item_position);
	}*/
}
