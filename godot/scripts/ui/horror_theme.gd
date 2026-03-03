extends Node

## Applies a dark, clinical horror theme to the entire UI at runtime.
## Autoloaded as "HorrorTheme" in project.godot.


func _ready() -> void:
	get_tree().root.theme = _build_theme()


func _build_theme() -> Theme:
	var t := Theme.new()
	t.default_font_size = 16

	# ── Color palette ──
	var bg_void := Color("06060c")
	var bg_panel := Color("0c0c18")
	var border_dark := Color("1a0c14")
	var border_accent := Color("381428")
	var crimson := Color("8b1a2b")
	var crimson_bright := Color("c4324a")
	var text_main := Color("c8bfbf")
	var text_dim := Color("605858")
	var text_dead := Color("302828")

	# ── Button ──
	t.set_stylebox("normal", "Button", _flat(Color("14101e"), border_dark, 3, 1, 12, 8))
	t.set_stylebox("hover", "Button", _flat(Color("1e1430"), border_accent, 3, 1, 12, 8))
	t.set_stylebox("pressed", "Button", _flat(Color("2a1028"), crimson, 3, 2, 12, 8))
	t.set_stylebox("disabled", "Button", _flat(Color("0a0a10"), Color("100c14"), 3, 1, 12, 8))
	t.set_stylebox("focus", "Button", _flat(Color("18102a"), crimson, 3, 2, 12, 8))
	t.set_color("font_color", "Button", text_main)
	t.set_color("font_hover_color", "Button", Color.WHITE)
	t.set_color("font_pressed_color", "Button", crimson_bright)
	t.set_color("font_disabled_color", "Button", text_dead)
	t.set_font_size("font_size", "Button", 15)

	# ── PanelContainer ──
	t.set_stylebox("panel", "PanelContainer", _flat(bg_panel, border_dark, 2, 1, 8, 8))

	# ── ProgressBar ──
	var pb_bg := _flat(Color("0a0a12"), border_dark, 2, 1, 0, 0)
	var pb_fill := _flat(crimson, Color("00000000"), 2, 0, 0, 0)
	t.set_stylebox("background", "ProgressBar", pb_bg)
	t.set_stylebox("fill", "ProgressBar", pb_fill)

	# ── Label ──
	t.set_color("font_color", "Label", text_main)
	t.set_font_size("font_size", "Label", 15)

	# ── RichTextLabel ──
	t.set_color("default_color", "RichTextLabel", text_dim)
	t.set_font_size("normal_font_size", "RichTextLabel", 14)

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
	t.set_stylebox("panel", "TooltipPanel", _flat(Color("0c0a14"), crimson, 3, 1, 10, 6))
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
