shader_type canvas_item;

uniform float timer : hint_range(0.0, 1.0, 0.01) = 0;

float box(vec2 size, float st) {
	vec2 _size = vec2(0.5) - size * 0.5;
	float _st = st;
	float x = smoothstep(_size.x, _size.x + 0.001, _st);
	float y = smoothstep(_size.y, _size.y + 0.001, _st);
    vec2 uv = vec2(x, y);
    uv *= smoothstep(_size,
        _size+vec2(0.001),
        vec2(1.0)-_st);
    return uv.x*uv.y;
}

void fragment() {
	float val = box(vec2(0.1, 0.8), 2);
	COLOR.a = val * timer;
}
