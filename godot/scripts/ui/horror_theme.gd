extends Node

## Applies a dark, clinical horror theme to the entire UI at runtime.
## Autoloaded as "HorrorTheme" in project.godot.


func _ready() -> void:
	get_tree().root.theme = _build_theme()


func _build_theme() -> Theme:
	var t := Theme.new()
	t.default_font_size = 18

	# ── Color palette (refined dark) ──
	@warning_ignore("unused_variable")
	var bg_void := Color("1a1a26")
	var bg_panel := Color("22222e")
	var border_dark := Color("3e3550")
	var border_accent := Color("6a3a58")
	var crimson := Color("c02848")
	var crimson_bright := Color("e85070")
	var text_main := Color("e8e2e2")
	var text_dim := Color("b0a8a8")
	var text_dead := Color("706868")

	# ── Button ──
	t.set_stylebox("normal", "Button", _flat(Color("28243a"), border_dark, 4, 1, 14, 10))
	t.set_stylebox("hover", "Button", _flat(Color("382e52"), border_accent, 4, 1, 14, 10))
	t.set_stylebox("pressed", "Button", _flat(Color("462850"), crimson, 4, 2, 14, 10))
	t.set_stylebox("disabled", "Button", _flat(Color("1e1a28"), Color("2e2838"), 4, 1, 14, 10))
	t.set_stylebox("focus", "Button", _flat(Color("32244a"), crimson, 4, 2, 14, 10))
	t.set_color("font_color", "Button", text_main)
	t.set_color("font_hover_color", "Button", Color.WHITE)
	t.set_color("font_pressed_color", "Button", crimson_bright)
	t.set_color("font_disabled_color", "Button", text_dead)
	t.set_font_size("font_size", "Button", 17)

	# ── PanelContainer ──
	t.set_stylebox("panel", "PanelContainer", _flat(bg_panel, border_dark, 4, 1, 10, 10))

	# ── ProgressBar ──
	var pb_bg := _flat(Color("1e1e2c"), border_dark, 3, 1, 0, 0)
	var pb_fill := _flat(crimson, Color("00000000"), 3, 0, 0, 0)
	t.set_stylebox("background", "ProgressBar", pb_bg)
	t.set_stylebox("fill", "ProgressBar", pb_fill)

	# ── Label ──
	t.set_color("font_color", "Label", text_main)
	t.set_font_size("font_size", "Label", 17)

	# ── RichTextLabel ──
	t.set_color("default_color", "RichTextLabel", text_dim)
	t.set_font_size("normal_font_size", "RichTextLabel", 16)

	# ── HSeparator ──
	var sep := StyleBoxLine.new()
	sep.color = border_dark
	sep.thickness = 1
	sep.content_margin_top = 4.0
	sep.content_margin_bottom = 4.0
	t.set_stylebox("separator", "HSeparator", sep)

	# ── ScrollContainer ──
	t.set_stylebox("panel", "ScrollContainer", _flat(Color("08081000"), Color("00000000"), 0, 0, 0, 0))

	# ── HSplitContainer ──
	t.set_constant("separation", "HSplitContainer", 16)

	# ── Tooltip ──
	t.set_stylebox("panel", "TooltipPanel", _flat(Color("201c30"), crimson, 4, 1, 10, 6))
	t.set_color("font_color", "TooltipLabel", text_main)

	return t


func _flat(bg: Color, border: Color, corner: int, bw: int, margin_h: float, margin_v: float) -> StyleBoxFlat:
	var s := StyleBoxFlat.new()
	s.bg_color = bg
	s.border_color = border
	s.corner_radius_top_left = corner
	s.corner_radius_top_right = corner
	s.corner_radius_bottom_left = corner
	s.corner_radius_bottom_right = corner
	s.border_width_left = bw
	s.border_width_top = bw
	s.border_width_right = bw
	s.border_width_bottom = bw
	s.content_margin_left = margin_h
	s.content_margin_right = margin_h
	s.content_margin_top = margin_v
	s.content_margin_bottom = margin_v
	return s
