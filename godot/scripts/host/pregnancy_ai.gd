class_name PregnancyAI
extends RefCounted

var _state: GameState
var _skill_tree_mgr: SkillTreeManager


func initialize(state: GameState, skill_tree_mgr: SkillTreeManager) -> void:
	_state = state
	_skill_tree_mgr = skill_tree_mgr


func choose_cards(state: GameState) -> Array[CardData]:
	_state = state
	var hand := state.pregnancy_deck.hand
	var budget := state.biomass
	var chosen: Array[CardData] = []

	# Simple priority AI: play highest-cost affordable cards first
	var sorted_hand := hand.duplicate()
	sorted_hand.sort_custom(_sort_by_cost_desc)

	var spent := 0.0
	for card in sorted_hand:
		if chosen.size() >= GameConstants.MAX_CARDS_PER_TURN:
			break
		if spent + card.play_cost <= budget:
			chosen.append(card)
			spent += card.play_cost

	# Also try to purchase skills if we have leftover biomass
	_try_purchase_skills(budget - spent)

	return chosen


func _try_purchase_skills(remaining_biomass: float) -> void:
	if remaining_biomass <= 0.0:
		return

	var trees := _skill_tree_mgr.get_pregnancy_trees()
	for tree in trees:
		for node in tree.nodes:
			if _skill_tree_mgr.can_purchase_pregnancy(node) and node.biomass_cost <= remaining_biomass:
				if _skill_tree_mgr.try_purchase_pregnancy(node):
					remaining_biomass -= node.biomass_cost
					if remaining_biomass <= 0.0:
						return


func _sort_by_cost_desc(a: CardData, b: CardData) -> bool:
	return a.play_cost > b.play_cost
