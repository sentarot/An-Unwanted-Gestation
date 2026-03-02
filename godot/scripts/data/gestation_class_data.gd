class_name GestationClassData
extends Resource

@export var class_name_text: String = ""
@export var class_type: int = Enums.GestationClassType.MACROSOMIC_MULTIPLES
@export_multiline var concept: String = ""
@export_multiline var physicality: String = ""

@export var gestation_speed_mult: float = 1.0
@export var base_gestation_cap: float = 100.0

@export var class_branch_a: SkillTreeData
@export var class_branch_b: SkillTreeData
@export var class_branch_c: SkillTreeData

@export var early_phase: PhaseVisualDescriptor
@export var mid_phase: PhaseVisualDescriptor
@export var late_phase: PhaseVisualDescriptor
@export var terminal_phase: PhaseVisualDescriptor
