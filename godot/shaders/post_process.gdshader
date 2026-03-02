shader_type canvas_item;

// Horror post-processing: vignette, chromatic aberration, color grading
uniform float vignette_intensity : hint_range(0.0, 1.0) = 0.3;
uniform float chromatic_aberration : hint_range(0.0, 0.02) = 0.0;
uniform float desaturation : hint_range(0.0, 1.0) = 0.2;
uniform vec4 tint_color : source_color = vec4(0.1, 0.0, 0.0, 1.0);
uniform float tint_strength : hint_range(0.0, 0.5) = 0.1;
uniform float pulse_speed : hint_range(0.0, 5.0) = 1.0;

void fragment() {
	vec2 uv = UV;
	vec4 color;

	// Chromatic aberration
	if (chromatic_aberration > 0.0) {
		float r = texture(TEXTURE, uv + vec2(chromatic_aberration, 0.0)).r;
		float g = texture(TEXTURE, uv).g;
		float b = texture(TEXTURE, uv - vec2(chromatic_aberration, 0.0)).b;
		color = vec4(r, g, b, 1.0);
	} else {
		color = texture(TEXTURE, uv);
	}

	// Vignette (pulsing)
	float dist = distance(uv, vec2(0.5));
	float pulse = 1.0 + sin(TIME * pulse_speed) * 0.1;
	float vignette = smoothstep(0.7, 0.3, dist * pulse);
	color.rgb *= mix(1.0, vignette, vignette_intensity);

	// Desaturation
	float grey = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	color.rgb = mix(color.rgb, vec3(grey), desaturation);

	// Horror tint
	color.rgb = mix(color.rgb, tint_color.rgb, tint_strength);

	COLOR = color;
}
