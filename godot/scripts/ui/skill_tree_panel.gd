extends PanelContainer

@onready var tree_container: VBoxContainer = %TreeContainer
@onready var tooltip_panel: PanelContainer = %TooltipPanel
@onready var tooltip_name: Label = %TooltipName
@onready var tooltip_desc: RichTextLabel = %TooltipDesc
@onready var tooltip_cost: Label = %TooltipCost
@onready var purchase_button: Button = %PurchaseButton

var _game_manager: Node
var _selected_node: SkillNodeData
var _all_node_buttons: Array[Dictionary] = []  # { "button": Button, "node": SkillNodeData }

const COLOR_LOCKED := Color(0.3, 0.3, 0.3)
const COLOR_AVAILABLE := Color(0.2, 0.7, 0.3)
const COLOR_PURCHASED := Color(0.8, 0.2, 0.4)
const COLOR_CANT_AFFORD := Color(0.6, 0.5, 0.1)


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.game_started.connect(_rebuild_tree)
	GameEvents.skill_purchased.connect(func(_n): _refresh_all_nodes())
	GameEvents.biomass_changed.connect(func(_v): _refresh_all_nodes())
	purchase_button.pressed.connect(_try_purchase_selected)
	_hide_tooltip()


func _rebuild_tree() -> void:
	_all_node_buttons.clear()

	# Clear existing children
	for child in tree_container.get_children():
		child.queue_free()

	var trees := _game_manager.skill_tree_mgr.get_all_trees()
	for tree in trees:
		_build_branch(tree)

	_hide_tooltip()


func _build_branch(tree: SkillTreeData) -> void:
	# Tree header
	var header := Label.new()
	header.text = "── %s ──" % tree.tree_name
	header.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	header.add_theme_color_override("font_color", Color(0.9, 0.7, 0.3))
	tree_container.add_child(header)

	var desc_label := Label.new()
	desc_label.text = tree.description
	desc_label.add_theme_font_size_override("font_size", 11)
	desc_label.add_theme_color_override("font_color", Color(0.6, 0.6, 0.6))
	tree_container.add_child(desc_label)

	# Nodes
	for node in tree.nodes:
		var btn := Button.new()
		btn.text = "%s [%d]" % [node.node_name, node.biomass_cost]
		btn.custom_minimum_size = Vector2(0, 32)
		btn.pressed.connect(_on_node_clicked.bind(node))
		tree_container.add_child(btn)

		_all_node_buttons.append({ "button": btn, "node": node })

	# Spacer
	var spacer := Control.new()
	spacer.custom_minimum_size = Vector2(0, 8)
	tree_container.add_child(spacer)


func _on_node_clicked(node: SkillNodeData) -> void:
	_selected_node = node
	_show_tooltip(node)


func _show_tooltip(node: SkillNodeData) -> void:
	tooltip_panel.show()
	tooltip_name.text = node.node_name
	tooltip_desc.text = node.description
	tooltip_cost.text = "COST: %d Biomass" % node.biomass_cost
	purchase_button.disabled = not _game_manager.skill_tree_mgr.can_purchase(node)


func _hide_tooltip() -> void:
	tooltip_panel.hide()
	_selected_node = null


func _try_purchase_selected() -> void:
	if _selected_node == null:
		return
	_game_manager.skill_tree_mgr.try_purchase(_selected_node)
	_refresh_all_nodes()
	_show_tooltip(_selected_node)


func _refresh_all_nodes() -> void:
	var state := _game_manager.state
	if state == null:
		return

	for entry in _all_node_buttons:
		var btn: Button = entry["button"]
		var node: SkillNodeData = entry["node"]

		var purchased := state.has_skill(node)
		var can_purchase := _game_manager.skill_tree_mgr.can_purchase(node)
		var prereqs_met := _are_prerequisites_met(node, state)

		if purchased:
			_set_button_style(btn, COLOR_PURCHASED)
			btn.disabled = true
		elif can_purchase:
			_set_button_style(btn, COLOR_AVAILABLE)
			btn.disabled = false
		elif prereqs_met and state.biomass < node.biomass_cost:
			_set_button_style(btn, COLOR_CANT_AFFORD)
			btn.disabled = false
		else:
			_set_button_style(btn, COLOR_LOCKED)
			btn.disabled = true


func _are_prerequisites_met(node: SkillNodeData, state: GameState) -> bool:
	if node.prerequisites.is_empty():
		return true
	for prereq in node.prerequisites:
		if not state.has_skill(prereq):
			return false
	return true


func _set_button_style(btn: Button, color: Color) -> void:
	var style := StyleBoxFlat.new()
	style.bg_color = color
	style.corner_radius_top_left = 4
	style.corner_radius_top_right = 4
	style.corner_radius_bottom_left = 4
	style.corner_radius_bottom_right = 4
	btn.add_theme_stylebox_override("normal", style)
