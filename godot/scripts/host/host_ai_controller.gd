class_name HostAIController
extends RefCounted

var _state: GameState
var _schedule: Array[Dictionary] = []


func initialize(state: GameState) -> void:
	_state = state
	_schedule = state.selected_host.daily_schedule


func process_tick(state: GameState) -> void:
	_state = state
	_evaluate_state()
	_execute_action()


func _evaluate_state() -> void:
	var old_state: int = _state.current_host_state

	# Mind-control check
	if _state.cancel_intervention:
		_state.is_mind_controlled = true

	# State transitions based on condition meters
	if _state.discomfort > GameConstants.BEDRIDDEN_DISCOMFORT_THRESHOLD:
		_state.current_host_state = Enums.HostState.BEDRIDDEN
		_state.mobility = 0.0
	elif _state.humiliation > GameConstants.ISOLATED_HUMILIATION_THRESHOLD:
		_state.current_host_state = Enums.HostState.ISOLATED
	else:
		_state.current_host_state = Enums.HostState.ACTIVE

	if _state.current_host_state != old_state:
		GameEvents.host_state_changed.emit(old_state, _state.current_host_state)
		var msg := ""
		match _state.current_host_state:
			Enums.HostState.BEDRIDDEN:
				msg = "%s collapses under the weight. She is bedridden." % _state.selected_host.host_name
			Enums.HostState.ISOLATED:
				msg = "%s refuses to leave the house. The humiliation is too great." % _state.selected_host.host_name
			Enums.HostState.ACTIVE:
				msg = "%s forces herself back into her routine." % _state.selected_host.host_name
		if msg != "":
			GameEvents.event_log_entry.emit(msg)


func _execute_action() -> void:
	match _state.current_host_state:
		Enums.HostState.ACTIVE:
			if _should_seek_intervention():
				_attempt_task(_create_intervention_entry())
			else:
				_attempt_scheduled_task()
		Enums.HostState.ISOLATED:
			if _should_seek_intervention():
				_attempt_task(_create_intervention_entry())
			else:
				_attempt_rest()
		Enums.HostState.BEDRIDDEN:
			_attempt_rest()


func _should_seek_intervention() -> bool:
	if _state.is_mind_controlled:
		return false
	if _state.gestation < _state.selected_host.panic_gestation_threshold:
		return false
	return _state.financial_resources > 5.0


func _attempt_scheduled_task() -> void:
	if _schedule.is_empty():
		return

	_state.schedule_index = _state.tick_within_day % _schedule.size()
	var entry: Dictionary = _schedule[_state.schedule_index]
	_attempt_task(entry)


func _attempt_task(entry: Dictionary) -> void:
	var success_chance := _calculate_success_chance(entry)
	var succeeded := randf() <= success_chance

	var task_type: int = entry["task_type"]
	if task_type == Enums.TaskType.SEEK_DOCTOR or task_type == Enums.TaskType.PRIVATE_CLINIC:
		if succeeded:
			var intervention_gain := GameConstants.INTERVENTION_PER_DOCTOR_VISIT \
				* _state.selected_host.intervention_drive \
				* (_state.financial_resources / 100.0)
			_state.intervention_meter = minf(100.0, _state.intervention_meter + intervention_gain)
			_state.financial_resources = maxf(0.0, _state.financial_resources - 5.0)
			GameEvents.intervention_changed.emit(_state.intervention_meter)
			GameEvents.event_log_entry.emit(
				"%s visits a specialist. Intervention progress: %.1f%%." % [
					_state.selected_host.host_name, _state.intervention_meter
				]
			)
		else:
			GameEvents.event_log_entry.emit(
				"%s attempts to see a doctor but fails. The condition interferes." % _state.selected_host.host_name
			)
	else:
		if succeeded:
			_apply_task_success(entry)
		else:
			_apply_task_failure(entry)

	GameEvents.host_task_resolved.emit(task_type, succeeded)


func _calculate_success_chance(entry: Dictionary) -> float:
	var chance: float = entry.get("base_success_chance", 0.85)

	# Penalties from host condition
	chance -= _state.discomfort * GameConstants.DISCOMFORT_TASK_PENALTY_MULT
	chance -= _state.humiliation * GameConstants.HUMILIATION_TASK_PENALTY_MULT

	# Penalty from player skills
	chance -= _state.task_failure_chance_bonus

	# Physical tasks penalized by mobility
	if entry.get("category", -1) == Enums.TaskCategory.PHYSICAL:
		chance *= _state.mobility

	# Social tasks penalized by humiliation
	if entry.get("category", -1) == Enums.TaskCategory.SOCIAL and _state.humiliation > 50.0:
		chance *= 0.6

	return clampf(chance, 0.0, 1.0)


func _apply_task_success(entry: Dictionary) -> void:
	_state.humiliation = maxf(0.0, _state.humiliation - 1.0)

	if entry.get("category", -1) == Enums.TaskCategory.SOCIAL:
		_state.social_standing = minf(100.0, _state.social_standing + 0.5)

	GameEvents.event_log_entry.emit(
		"%s completes: %s." % [_state.selected_host.host_name, entry.get("task_label", "Unknown")]
	)


func _apply_task_failure(entry: Dictionary) -> void:
	var hum_gain := 3.0
	var dis_gain := 2.0

	# Vulnerability amplification
	if _state.selected_host.vulnerability == Enums.VulnerabilityType.HUMILIATION:
		hum_gain *= _state.selected_host.vulnerability_multiplier
	if _state.selected_host.vulnerability == Enums.VulnerabilityType.DISCOMFORT:
		dis_gain *= _state.selected_host.vulnerability_multiplier

	_state.humiliation = clampf(_state.humiliation + hum_gain, 0.0, 100.0)
	_state.discomfort = clampf(_state.discomfort + dis_gain, 0.0, 100.0)
	_state.social_standing = maxf(0.0, _state.social_standing - 1.0)

	GameEvents.humiliation_changed.emit(_state.humiliation)
	GameEvents.discomfort_changed.emit(_state.discomfort)
	GameEvents.social_standing_changed.emit(_state.social_standing)

	GameEvents.event_log_entry.emit(
		"%s fails: %s. Humiliation +%.0f, Discomfort +%.0f." % [
			_state.selected_host.host_name, entry.get("task_label", "Unknown"),
			hum_gain, dis_gain
		]
	)


func _attempt_rest() -> void:
	_state.discomfort = maxf(0.0, _state.discomfort - 1.5)
	GameEvents.discomfort_changed.emit(_state.discomfort)


func _create_intervention_entry() -> Dictionary:
	return {
		"task_label": "Seek Medical Intervention",
		"task_type": Enums.TaskType.SEEK_DOCTOR,
		"category": Enums.TaskCategory.INTERVENTION,
		"base_success_chance": 0.7,
		"primary_stat": Enums.StatAffinity.FINANCIAL
	}
