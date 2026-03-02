class_name SkillTreeManager
extends RefCounted

var _state: GameState
var _data_factory: DataFactory

var somatic_tree: SkillTreeData
var endocrine_tree: SkillTreeData
var temporal_tree: SkillTreeData


func initialize(state: GameState, factory: DataFactory) -> void:
	_state = state
	_data_factory = factory
	somatic_tree = factory.general_somatic
	endocrine_tree = factory.general_endocrine
	temporal_tree = factory.general_temporal


func can_purchase(node: SkillNodeData) -> bool:
	if _state == null or _state.is_game_over:
		return false
	if _state.has_skill(node):
		return false
	if _state.biomass < node.biomass_cost:
		return false

	# Check prerequisites
	for prereq in node.prerequisites:
		if not _state.has_skill(prereq):
			return false

	return true


func try_purchase(node: SkillNodeData) -> bool:
	if not can_purchase(node):
		return false

	_state.biomass -= node.biomass_cost
	_state.purchased_skills.append(node)

	# Apply one-time effects
	for effect in node.effects:
		if not effect.get("is_per_tick", true):
			_apply_one_time_effect(effect)

	GameEvents.biomass_changed.emit(_state.biomass)
	GameEvents.skill_purchased.emit(node)

	if node.activation_narrative != "":
		GameEvents.event_log_entry.emit(node.activation_narrative)

	return true


func _apply_one_time_effect(effect: Dictionary) -> void:
	var etype: int = effect["type"]
	var mag: float = effect["magnitude"]
	var E := Enums.SkillEffectType

	match etype:
		E.RAISE_GESTATION_CAP:
			_state.gestation_cap = maxf(_state.gestation_cap, mag)
		E.REDUCE_INTELLECT:
			_state.mental_defense = maxf(0.0, _state.mental_defense * (1.0 - mag))
		E.REDUCE_SOCIAL_STANDING:
			_state.social_standing = maxf(0.0, _state.social_standing - mag)
			GameEvents.social_standing_changed.emit(_state.social_standing)
		E.DELETE_AUTHORITY:
			_state.mental_defense *= 0.1
			_state.social_standing *= 0.5
		E.REPLACE_TASK:
			_state.is_mind_controlled = true


func get_all_trees() -> Array[SkillTreeData]:
	var trees: Array[SkillTreeData] = [somatic_tree, endocrine_tree, temporal_tree]
	var cls := _state.selected_class
	if cls.class_branch_a:
		trees.append(cls.class_branch_a)
	if cls.class_branch_b:
		trees.append(cls.class_branch_b)
	if cls.class_branch_c:
		trees.append(cls.class_branch_c)
	return trees
