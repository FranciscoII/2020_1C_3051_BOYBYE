/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float3 eyePosition;

struct Light
{
    float3 position;
    float3 color;
};

Light lights[2];

float screenWidth, screenHeight, timer = 0.0;

static const int kernelRadius = 5;
static const int kernelSize = 25;
static const float kernel[kernelSize] =
{
    0.003765, 0.015019, 0.023792, 0.015019, 0.003765,
    0.015019, 0.059912, 0.094907, 0.059912, 0.015019,
    0.023792, 0.094907, 0.150342, 0.094907, 0.023792,
    0.015019, 0.059912, 0.094907, 0.059912, 0.015019,
    0.003765, 0.015019, 0.023792, 0.015019, 0.003765,
};


//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Textura para full screen quad
texture renderTarget;
sampler2D renderTargetSampler = sampler_state
{
    Texture = (renderTarget);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};










//Input del Vertex Shader
struct VS_INPUT_DEFAULT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_DEFAULT VSDefault(VS_INPUT_DEFAULT input)
{
    VS_OUTPUT_DEFAULT output;

	// Enviamos la posicion transformada
    output.Position = mul(input.Position, matWorldViewProj);
    
    // Propagar las normales por la matriz normal
    output.Normal = mul(input.Normal, matInverseTransposeWorld);
    
	// Propagar coordenadas de textura
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

//Pixel Shader
float4 PSDefault(VS_OUTPUT_DEFAULT input) : COLOR0
{
    return tex2D(diffuseMap, input.TextureCoordinates);
}





//Input del Vertex Shader
struct VS_INPUT_POSTPROCESS
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_POSTPROCESS
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_POSTPROCESS VSPostProcess(VS_INPUT_POSTPROCESS input)
{
    VS_OUTPUT_POSTPROCESS output;

	// Propagamos la posicion, ya que esta en espacio de pantalla
    output.Position = input.Position;

	// Propagar coordenadas de textura
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

//Pixel Shader
float4 PSPostProcess(VS_OUTPUT_DEFAULT input) : COLOR0
{
    float distanciaAlBorde = distance(input.TextureCoordinates, float2(0.5, 0.5));
    float4 tex = tex2D(renderTargetSampler, input.TextureCoordinates);
    
    int pixelX = round(input.TextureCoordinates.x * screenWidth);
    int pixelY = round(input.TextureCoordinates.y * screenHeight);
    
    int cuadrillaX = fmod(pixelX, 3.0);
    int cuadrillaY = fmod(pixelY, 3.0);
    
    tex = (cuadrillaX <= 1.0) * (cuadrillaY <= 1.0) * tex;
    tex.g += 0.2;
    
    return lerp(tex, float4(0, 0, 0, 1), distanciaAlBorde * 1.3);

}






technique Default
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSDefault();
        PixelShader = compile ps_3_0 PSDefault();
    }
}

technique PostProcess
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSPostProcess();
        PixelShader = compile ps_3_0 PSPostProcess();
    }
}
//------------------------



//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT VSEjercicio2(VS_INPUT input)
{
    VS_OUTPUT output;
    float3 centro = float3(0, 0, 0);
    
    float distancia = distance(input.Position.xyz, centro);
    
    float4 posicionEsfera = float4(input.Position.xyz * 10.0 * abs(sin(timer)) / distancia, input.Position.w);
    
    float4 posicionInterpolada = lerp(input.Position, posicionEsfera, abs(sin(timer)));
	// Enviamos la posicion transformada
    output.Position = mul(posicionInterpolada, matWorldViewProj);
    
    // Propagar las normales por la matriz normal
    output.Normal = mul(input.Normal, matInverseTransposeWorld);
    
	// Propagar coordenadas de textura
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

//Pixel Shader
float4 PSEjercicio2(VS_OUTPUT input) : COLOR0
{
    float4 tex = tex2D(diffuseMap, input.TextureCoordinates);
    float4 colorAzul = float4(0, 0, 1, 1);
    return lerp(tex, colorAzul, abs(sin(timer)));

}

technique Ejercicio2
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 VSEjercicio2();
        PixelShader = compile ps_3_0 PSEjercicio2();
    }
}