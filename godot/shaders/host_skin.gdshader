shader_type canvas_item;

// Host character shader: thermal flush, sweat, vein glow, stretch marks, translucency
uniform float thermal_flush : hint_range(0.0, 1.0) = 0.0;
uniform float sweat_glisten : hint_range(0.0, 1.0) = 0.0;
uniform float vein_glow : hint_range(0.0, 1.0) = 0.0;
uniform vec4 vein_color : source_color = vec4(0.3, 0.8, 0.4, 1.0);
uniform float skin_translucency : hint_range(0.0, 1.0) = 0.0;
uniform float stretch_marks : hint_range(0.0, 1.0) = 0.0;
uniform float belly_scale : hint_range(1.0, 4.0) = 1.0;

void fragment() {
	vec4 tex = texture(TEXTURE, UV);

	// Thermal flush (red tint to skin)
	if (thermal_flush > 0.0) {
		tex.r = min(1.0, tex.r + thermal_flush * 0.3);
		tex.g *= 1.0 - thermal_flush * 0.15;
		tex.b *= 1.0 - thermal_flush * 0.2;
	}

	// Sweat glisten (specular highlight simulation)
	if (sweat_glisten > 0.0) {
		float spec = pow(max(0.0, dot(normalize(vec2(UV.x - 0.5, UV.y - 0.3)), vec2(0.5, 0.5))), 8.0);
		tex.rgb += vec3(spec * sweat_glisten * 0.4);
	}

	// Vein glow (emissive overlay pulsing)
	if (vein_glow > 0.0) {
		float vein_pattern = sin(UV.x * 20.0 + TIME * 2.0) * sin(UV.y * 15.0 + TIME * 1.5);
		vein_pattern = smoothstep(0.3, 0.7, vein_pattern * 0.5 + 0.5);
		float pulse = (sin(TIME * 3.0) * 0.5 + 0.5) * vein_glow;
		tex.rgb = mix(tex.rgb, vein_color.rgb, vein_pattern * pulse * 0.6);
	}

	// Skin translucency (subsurface scattering approximation)
	if (skin_translucency > 0.0) {
		float sss = pow(1.0 - abs(UV.x - 0.5) * 2.0, 2.0) * skin_translucency;
		tex.rgb += vec3(0.9, 0.3, 0.2) * sss * 0.3;
		tex.a = mix(tex.a, tex.a * 0.85, skin_translucency);
	}

	// Stretch marks (darkening pattern)
	if (stretch_marks > 0.0) {
		float marks = sin(UV.y * 40.0 + UV.x * 5.0) * sin(UV.x * 30.0);
		marks = smoothstep(0.4, 0.6, marks * 0.5 + 0.5);
		tex.rgb *= 1.0 - marks * stretch_marks * 0.25;
	}

	COLOR = tex;
}
