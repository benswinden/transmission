using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
 
public class LineDisplay : MonoBehaviour {

	//line drawing
	private VectorLine Line;
	private List<Vector3> LinePoints;

	//width of line
	public float LineWidth = 1f;

	void Awake() {
		//initial line points
		LinePoints = new List<Vector3>() {Vector3.zero, Vector3.zero};
		//create vectrosity line
		Line = new VectorLine("Line", LinePoints, LineWidth, LineType.Continuous);
	}
 
	public void DisplayLine(List<Vector3> linePoints) {
		//clear line list
		LinePoints.Clear();
		//add new points to line list
		for(int i = 0; i < linePoints.Count; i++){
			LinePoints.Add(linePoints[i]);
		}
		//create line if it was destroyed
		if(Line == null){
			Line = new VectorLine("Line", LinePoints, LineWidth, LineType.Continuous);
			Line.active = true;
		}
		//draw line
		Line.Draw3D();

	}

	public void ClearLine() {
		//clear point list
		LinePoints.Clear();
		if (Line != null) {
			Line.Draw3D();
			Line.active = false;
		}
		//kill line
		VectorLine.Destroy(ref Line);
	}

	public bool LineActive{
		get{
			return LinePoints.Count > 0;
		}
	}

 }