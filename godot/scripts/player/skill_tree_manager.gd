class_name SkillTreeManager
extends RefCounted

var _state: GameState
var _data_factory: DataFactory

# Pregnancy skill trees
var somatic_tree: SkillTreeData
var endocrine_tree: SkillTreeData
var temporal_tree: SkillTreeData

# Host skill trees
var host_self_care_tree: SkillTreeData
var host_investigation_tree: SkillTreeData
var host_medical_tree: SkillTreeData
var host_social_tree: SkillTreeData


func initialize(state: GameState, factory: DataFactory) -> void:
	_state = state
	_data_factory = factory
	somatic_tree = factory.general_somatic
	endocrine_tree = factory.general_endocrine
	temporal_tree = factory.general_temporal
	host_self_care_tree = factory.host_self_care
	host_investigation_tree = factory.host_investigation
	host_medical_tree = factory.host_medical
	host_social_tree = factory.host_social


func can_purchase_pregnancy(node: SkillNodeData) -> bool:
	if _state == null or _state.is_game_over:
		return false
	if node in _state.purchased_skills:
		return false
	if _state.biomass < node.biomass_cost:
		return false
	for prereq in node.prerequisites:
		if prereq not in _state.purchased_skills:
			return false
	return true


func try_purchase_pregnancy(node: SkillNodeData) -> bool:
	if not can_purchase_pregnancy(node):
		return false

	_state.biomass -= node.biomass_cost
	_state.purchased_skills.append(node)

	for effect in node.effects:
		if not effect.get("is_per_tick", true):
			_apply_one_time_pregnancy_effect(effect)

	GameEvents.biomass_changed.emit(_state.biomass)
	GameEvents.skill_purchased.emit(node)

	if node.activation_narrative != "":
		GameEvents.event_log_entry.emit(node.activation_narrative)
	return true


func can_purchase_host(node: SkillNodeData) -> bool:
	if _state == null or _state.is_game_over:
		return false
	if node in _state.purchased_host_skills:
		return false
	if _state.awareness < node.biomass_cost:
		return false
	for prereq in node.prerequisites:
		if prereq not in _state.purchased_host_skills:
			return false
	return true


func try_purchase_host(node: SkillNodeData) -> bool:
	if not can_purchase_host(node):
		return false

	_state.awareness -= node.biomass_cost
	_state.purchased_host_skills.append(node)

	for effect in node.effects:
		if not effect.get("is_per_tick", true):
			_apply_one_time_host_effect(effect)

	GameEvents.awareness_changed.emit(_state.awareness)
	GameEvents.skill_purchased.emit(node)

	if node.activation_narrative != "":
		GameEvents.event_log_entry.emit(node.activation_narrative)
	return true


# Legacy compatibility
func can_purchase(node: SkillNodeData) -> bool:
	return can_purchase_pregnancy(node)

func try_purchase(node: SkillNodeData) -> bool:
	return try_purchase_pregnancy(node)


func _apply_one_time_pregnancy_effect(effect: Dictionary) -> void:
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


func _apply_one_time_host_effect(effect: Dictionary) -> void:
	var etype: int = effect["type"]
	var mag: float = effect["magnitude"]
	var E := Enums.SkillEffectType

	match etype:
		E.BOOST_INTERVENTION:
			_state.intervention_meter = minf(100.0, _state.intervention_meter + mag)
			GameEvents.intervention_changed.emit(_state.intervention_meter)
		E.REDUCE_BIOMASS:
			_state.biomass = maxf(0.0, _state.biomass - mag)
			GameEvents.biomass_changed.emit(_state.biomass)


func get_pregnancy_trees() -> Array[SkillTreeData]:
	var trees: Array[SkillTreeData] = [somatic_tree, endocrine_tree, temporal_tree]
	var cls := _state.selected_class
	if cls.class_branch_a:
		trees.append(cls.class_branch_a)
	if cls.class_branch_b:
		trees.append(cls.class_branch_b)
	if cls.class_branch_c:
		trees.append(cls.class_branch_c)
	return trees


func get_host_trees() -> Array[SkillTreeData]:
	return [host_self_care_tree, host_investigation_tree, host_medical_tree, host_social_tree]


func get_all_trees() -> Array[SkillTreeData]:
	var trees := get_pregnancy_trees()
	trees.append_array(get_host_trees())
	return trees
