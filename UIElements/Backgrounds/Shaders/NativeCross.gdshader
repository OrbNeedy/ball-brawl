shader_type spatial;

uniform sampler2D noise;
uniform sampler2D border_map;
uniform float alpha = 1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	ALBEDO = texture(noise, SCREEN_UV).rgb;
	ALPHA = texture(border_map, UV).a * alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
