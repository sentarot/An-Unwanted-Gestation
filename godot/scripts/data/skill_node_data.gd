class_name SkillNodeData
extends Resource

@export var node_name: String = ""
@export var tier: int = 1
@export_multiline var description: String = ""
@export var biomass_cost: int = 0
@export var prerequisites: Array[SkillNodeData] = []
@export var effects: Array[Dictionary] = []
# Each effect dict: { "type": SkillEffectType, "magnitude": float, "is_per_tick": bool }
@export_multiline var activation_narrative: String = ""
@export_multiline var tick_narrative: String = ""


static func create(p_name: String, p_tier: int, p_cost: int, p_desc: String, p_effects: Array[Dictionary]) -> SkillNodeData:
	var node := SkillNodeData.new()
	node.node_name = p_name
	node.tier = p_tier
	node.biomass_cost = p_cost
	node.description = p_desc
	node.effects = p_effects
	node.activation_narrative = ">> %s activated." % p_name
	return node
