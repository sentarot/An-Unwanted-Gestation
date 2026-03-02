extends PanelContainer

@onready var log_text: RichTextLabel = %LogText
@onready var scroll_container: ScrollContainer = %LogScrollContainer

var _lines: PackedStringArray = PackedStringArray()
var _max_lines: int = 50
var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.event_log_entry.connect(_add_entry)


func _add_entry(text: String) -> void:
	var timestamp := ""
	if _game_manager and _game_manager.state:
		var state := _game_manager.state
		timestamp = "[D%d:%02d] " % [state.current_day, state.tick_within_day]

	_lines.append(timestamp + text)
	while _lines.size() > _max_lines:
		_lines.remove_at(0)

	log_text.text = "\n".join(_lines)

	# Auto-scroll to bottom
	await get_tree().process_frame
	if scroll_container:
		scroll_container.scroll_vertical = scroll_container.get_v_scroll_bar().max_value as int
