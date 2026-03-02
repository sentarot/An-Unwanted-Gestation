class_name SkillTreeData
extends Resource

@export var tree_name: String = ""
@export var branch: int = Enums.SkillBranch.SOMATIC_MALADY
@export_multiline var description: String = ""
@export var nodes: Array[SkillNodeData] = []
