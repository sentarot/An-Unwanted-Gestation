class_name HostAI
extends RefCounted

var _state: GameState
var _skill_tree_mgr: SkillTreeManager


func initialize(state: GameState, skill_tree_mgr: SkillTreeManager) -> void:
	_state = state
	_skill_tree_mgr = skill_tree_mgr


func choose_cards(state: GameState) -> Array[CardData]:
	_state = state
	var hand := state.host_deck.hand
	var budget := state.awareness
	var chosen: Array[CardData] = []

	# Priority: if discomfort is high, prioritize healing cards
	# If gestation is high, prioritize intervention cards
	# Otherwise, play awareness/general cards
	var sorted_hand := hand.duplicate()
	sorted_hand.sort_custom(_sort_by_priority)

	var spent := 0.0
	for card in sorted_hand:
		if chosen.size() >= GameConstants.MAX_CARDS_PER_TURN:
			break
		if spent + card.play_cost <= budget:
			chosen.append(card)
			spent += card.play_cost

	# Try to purchase host skills with leftover awareness
	_try_purchase_skills(budget - spent)

	return chosen


func _try_purchase_skills(remaining_awareness: float) -> void:
	if remaining_awareness <= 0.0:
		return

	var trees := _skill_tree_mgr.get_host_trees()
	# Prioritize investigation and medical when gestation is high
	if _state.gestation > GameConstants.PANIC_GESTATION_THRESHOLD:
		trees.sort_custom(func(a: SkillTreeData, b: SkillTreeData) -> bool:
			var a_priority := 0
			var b_priority := 0
			if a.branch == Enums.SkillBranch.HOST_MEDICAL or a.branch == Enums.SkillBranch.HOST_INVESTIGATION:
				a_priority = 1
			if b.branch == Enums.SkillBranch.HOST_MEDICAL or b.branch == Enums.SkillBranch.HOST_INVESTIGATION:
				b_priority = 1
			return a_priority > b_priority
		)

	for tree in trees:
		for node in tree.nodes:
			if _skill_tree_mgr.can_purchase_host(node) and node.biomass_cost <= remaining_awareness:
				if _skill_tree_mgr.try_purchase_host(node):
					remaining_awareness -= node.biomass_cost
					if remaining_awareness <= 0.0:
						return


func _sort_by_priority(a: CardData, b: CardData) -> bool:
	var a_score := _card_priority_score(a)
	var b_score := _card_priority_score(b)
	return a_score > b_score


func _card_priority_score(card: CardData) -> float:
	var score := 0.0
	var E := Enums.SkillEffectType

	for effect in card.effects:
		var etype: int = effect["type"]
		var mag: float = effect["magnitude"]
		match etype:
			E.BOOST_INTERVENTION:
				score += mag * 2.0
				if _state.gestation > 50.0:
					score += mag * 3.0  # Urgent when gestation is high
			E.REDUCE_GESTATION:
				score += mag * 3.0
			E.ADD_DISCOMFORT:
				if mag < 0:  # Healing
					var urgency := _state.discomfort / 100.0
					score += absf(mag) * (1.0 + urgency * 2.0)
			E.ADD_HUMILIATION:
				if mag < 0:  # Healing
					var urgency := _state.humiliation / 100.0
					score += absf(mag) * (1.0 + urgency * 2.0)
			E.RAISE_AWARENESS:
				score += mag * 1.5
			E.REDUCE_BIOMASS:
				score += mag * 2.0

	return score
