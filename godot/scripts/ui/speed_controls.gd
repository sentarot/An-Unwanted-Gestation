extends HBoxContainer

# Speed controls are a no-op in versus/turn-based mode.
# Kept for UI compatibility; buttons do nothing harmful.

@onready var pause_button: Button = %PauseButton
@onready var speed1_button: Button = %Speed1Button
@onready var speed2_button: Button = %Speed2Button
@onready var speed4_button: Button = %Speed4Button
@onready var speed_label: Label = %SpeedLabel

var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	if speed_label:
		speed_label.text = "TURN-BASED"
