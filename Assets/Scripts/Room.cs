using UnityEngine;
using System.Collections;

public class Room{

	public int top_x;
	/*Since we are in 3D space y is the vertical and the plain is on the x z. I am treating Z as the y axis like I would in console rogue.*/
	public int top_z;
	public int width_x;
	public int length_z; /*would be height if using y in 2D space*/

	public Room(int top_x_coordinate, int top_z_coordinate,int width,int length){

		top_x = top_x_coordinate;
		top_z = top_z_coordinate;

		width_x = width;
		length_z = length;
	}
}
