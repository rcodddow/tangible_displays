using UnityEngine;
using System.Collections;

public static class UTIL_Utilities{
	//Highlight Variables	////////////
	public static Material PartSelectionOutline=Resources.Load("Materials/PartSelectionOutline", typeof(Material)) as Material;
	public static Material PartSelectionHighlight=Resources.Load("Materials/PartSelectionHighlight", typeof(Material)) as Material;

	public static void AddHighlight(GameObject _object, bool _children=true){
		//Adds a highlight to a GameObject. Includes a transparent cloud, a toon outline and a UV moving script.
		//_children determines whether the children also get the highlight.
		if(_object.renderer!=null){
			Material[] newMats=new Material[_object.renderer.materials.Length+2];	//Create a new array of materials.

			//Copy over any old materials to the new array.
			for(int i=0; i<newMats.Length-2; i++)	
				newMats[i]=_object.renderer.materials[i];

			//Add the PartSelectionOutline and PartSelectionHighlight.
			newMats[newMats.Length-2]=PartSelectionOutline;
			newMats[newMats.Length-1]=PartSelectionHighlight;

			//Add the UVMover script.
			UTIL_UVMover tmpUVMover=_object.AddComponent<UTIL_UVMover>();
			tmpUVMover.matIndex=newMats.Length-1;	//Index of the highlight material.

			//Set the new materials on the object.
			_object.renderer.materials=newMats;
		}
		if(_children){
			for(int i=0; i<_object.transform.childCount; i++)
				AddHighlight(_object.transform.GetChild(i).gameObject, _children);
		}
	}
	public static void RemoveHighlight(GameObject _object, bool _children=true){
		//Removes the highlight added to a GameObject from "AddHighlight".
		bool outline=false;		//Whether or not the outline and the highlight are on the object.
		bool highlight=false;
		int outlineIndex=-1;	//Indices into the object's materials for which materials are the outline and highlight.
		int highlightIndex=-1;

		//Remove the UVMover script.
		if(_object.GetComponent<UTIL_UVMover>()!=null)
			UnityEngine.Object.Destroy(_object.GetComponent<UTIL_UVMover>());

		if(_object.renderer!=null){
			//Check which materials are already attached to the object.
			for(int i=0; i<_object.renderer.materials.Length; i++){
				if(_object.renderer.materials[i].name.Contains(PartSelectionOutline.name)){
					outline=true;
					outlineIndex=i;
				}
				if(_object.renderer.materials[i].name.Contains(PartSelectionHighlight.name)){
					highlight=true;
					highlightIndex=i;
				}
			}

			//Determine how many new materials the object will have based on whether or not the outline or highlight materials are already there.
			int newMaterialsLength;
			if((outline)&&(highlight)) newMaterialsLength=_object.renderer.materials.Length-2;
			else if((outline)||(highlight)) newMaterialsLength=_object.renderer.materials.Length-1;
			else return;	//Neither the highlight or the outline are attached, return.

			Material [] newMats=new Material[newMaterialsLength];	//Create a new array of materials.
		
			//Copy over the old materials to the new array.
			int lastCopiedIndex=0;
			for(int i=0; i<_object.renderer.materials.Length; i++){
				if((i!=outlineIndex)&&(i!=highlightIndex)){
					newMats[lastCopiedIndex]=_object.renderer.materials[i];
					lastCopiedIndex++;
				}
			}

			//Set the new materials on the object.
			_object.renderer.materials=newMats;
		}
		if(_children)
			for(int i=0; i<_object.transform.childCount; i++)
				RemoveHighlight(_object.transform.GetChild(i).transform.gameObject, _children);
	}
}
