using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorState : StateMachineBehaviour {
	public int minTemplates = 5;
	public int maxTemplates = 10;
	[Range(0f, 1f)]
	public float uniqueChance = 0.5f;

	public TerrainTemplate[] templates;
	public TerrainTemplate[] uniqueTemplates;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        RiverGenerator riverGenerator = GameController.GetInstance().riverGenerator;
		riverGenerator.RepopulateBuffer(templates, uniqueTemplates, Random.Range(minTemplates, maxTemplates), uniqueChance);
	}
}
