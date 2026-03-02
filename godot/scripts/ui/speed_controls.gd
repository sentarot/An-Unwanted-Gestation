extends HBoxContainer

@onready var pause_button: Button = %PauseButton
@onready var speed1_button: Button = %Speed1Button
@onready var speed2_button: Button = %Speed2Button
@onready var speed4_button: Button = %Speed4Button
@onready var speed_label: Label = %SpeedLabel

var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	pause_button.pressed.connect(_on_pause)
	speed1_button.pressed.connect(_set_speed.bind(1.0))
	speed2_button.pressed.connect(_set_speed.bind(2.0))
	speed4_button.pressed.connect(_set_speed.bind(4.0))


func _on_pause() -> void:
	if _game_manager == null:
		return

	if _game_manager.is_paused:
		_game_manager.resume()
		_update_label(_game_manager.speed_multiplier)
	else:
		_game_manager.pause()
		_update_label(0.0)


func _set_speed(mult: float) -> void:
	if _game_manager == null:
		return
	_game_manager.set_speed(mult)
	if _game_manager.is_paused:
		_game_manager.resume()
	_update_label(mult)


func _update_label(mult: float) -> void:
	if speed_label == null:
		return
	speed_label.text = "PAUSED" if mult <= 0.0 else "%dx" % int(mult)
