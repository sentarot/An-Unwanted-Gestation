class_name PhaseVisualDescriptor
extends Resource

@export var phase: int = Enums.GestationPhase.EARLY
@export var gestation_min: float = 0.0
@export var gestation_max: float = 100.0
@export_multiline var visual_description: String = ""
@export var belly_scale_mult: float = 1.0
@export var posture_swayback: float = 0.0
@export var move_speed_mult: float = 1.0


static func create(p_phase: int, p_min: float, p_max: float, p_desc: String,
		p_belly: float, p_posture: float, p_speed: float) -> PhaseVisualDescriptor:
	var d := PhaseVisualDescriptor.new()
	d.phase = p_phase
	d.gestation_min = p_min
	d.gestation_max = p_max
	d.visual_description = p_desc
	d.belly_scale_mult = p_belly
	d.posture_swayback = p_posture
	d.move_speed_mult = p_speed
	return d
