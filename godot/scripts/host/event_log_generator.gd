class_name EventLogGenerator
extends RefCounted

var _game_manager: Node
var _last_milestone: float = 0.0


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.skill_purchased.connect(_on_skill_purchased)
	GameEvents.host_state_changed.connect(_on_host_state_changed)
	GameEvents.gestation_changed.connect(_on_gestation_milestone)


func _get_state() -> GameState:
	if _game_manager and _game_manager.state:
		return _game_manager.state
	return null


func _on_skill_purchased(node: Resource) -> void:
	var state := _get_state()
	if state == null:
		return

	for effect in node.effects:
		var narrative := _generate_effect_narrative(effect, node, state)
		if narrative != "":
			GameEvents.event_log_entry.emit(narrative)


func _on_host_state_changed(old_state: int, new_state: int) -> void:
	var state := _get_state()
	if state == null:
		return

	if new_state == Enums.HostState.BEDRIDDEN and state.has_visual_flag(Enums.SkillEffectType.VISUAL_SKIN_TRANSLUCENT):
		GameEvents.event_log_entry.emit(
			"The taut, translucent skin of %s's abdomen shines under the fluorescent lights as she lies still." % state.selected_host.host_name
		)


func _on_gestation_milestone(gestation: float) -> void:
	var state := _get_state()
	if state == null:
		return

	if gestation >= 15.0 and _last_milestone < 15.0:
		GameEvents.event_log_entry.emit(
			"Phase shift: %s's condition is now visibly apparent." % state.selected_host.host_name
		)
		_last_milestone = 15.0
	elif gestation >= 41.0 and _last_milestone < 41.0:
		GameEvents.event_log_entry.emit(
			"Phase shift: %s can no longer conceal her state. The burden is undeniable." % state.selected_host.host_name
		)
		_last_milestone = 41.0
	elif gestation >= 76.0 and _last_milestone < 76.0:
		GameEvents.event_log_entry.emit(
			"TERMINAL PHASE: %s has entered the final stage. Every step is a labored ordeal." % state.selected_host.host_name
		)
		_last_milestone = 76.0


func _generate_effect_narrative(effect: Dictionary, node: Resource, state: GameState) -> String:
	var host: String = state.selected_host.host_name
	var etype: int = effect.get("type", -1)
	var E := Enums.SkillEffectType

	match etype:
		E.VISUAL_SWEAT:
			return "A permanent sheen of perspiration covers %s's skin. She dabs at her brow constantly, but the glistening never fades." % host
		E.VISUAL_WARDROBE_FAILURE:
			return "A button gives way on %s's blouse with an audible pop. Her wardrobe can no longer contain the expansion." % host
		E.VISUAL_ABDOMINAL_UNDULATION:
			return "Something shifts violently beneath the taut surface of %s's abdomen. She gasps and clutches her midsection as the movement ripples outward." % host
		E.VISUAL_GLOWING_VEINS:
			return "Faint, luminous veins trace patterns across %s's distended belly, pulsing with an otherworldly rhythm." % host
		E.VISUAL_SKIN_TRANSLUCENT:
			return "The skin of %s's abdomen becomes impossibly taut and translucent, stretched like a drum over the massive burden within." % host
		E.VISUAL_PETITIFICATION:
			return "%s's frame seems to shrink around the growing burden. Her limbs thin, her shoulders narrow — only the belly remains immense." % host
		E.VISUAL_SPHERICAL_BELLY:
			return "%s's abdomen rounds into a perfect, taut sphere — geometrically flawless and impossibly large." % host
		E.CANCEL_INTERVENTION:
			return "A wave of chemically induced calm washes over %s. The thought of seeking help dissolves into warm compliance." % host
		E.DELETE_AUTHORITY:
			return "NPCs no longer address %s by name. Their eyes track only the immense curvature of her midsection." % host

	return ""
