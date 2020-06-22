/*
* Shader generico para TgcMesh.
* Hay 3 Techniques, una para cada MeshRenderType:
*	- VERTEX_COLOR
*	- DIFFUSE_MAP
*	- DIFFUSE_MAP_AND_LIGHTMAP
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
float3 posicionNave;
float3 posicionSol;
//Otras variables para la luz de la nave
float3 ambientColor; //Color RGB para Ambient de la luz
float3 diffuseColor; //Color RGB para Diffuse de la luz
float3 specularColor; //Color RGB para Specular de la luz
float KAmbient; // Coeficiente de Ambient
float KDiffuse; // Coeficiente de Diffuse
float KSpecular; // Coeficiente de Specular
float shininess; //Exponente de specular
float3 lightPosition; //Posicion de la luz
float3 eyePosition; //Posicion de la camara

float tiempo; //Utilizado para nave en game over


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

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
	Texture = (texLightMap);
};

/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
    float3 worldPosition : TEXCOORD2;
    float3 worldNormal : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT vertexShader(VS_INPUT input)
{
	VS_OUTPUT output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

    output.worldPosition = mul(input.Position, matWorld);
    output.worldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	return output;
}

//Input del Pixel Shader
struct PS
{
	float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
    float3 worldPosition : TEXCOORD2;
};

//Pixel Shader
float4 pixelShader(VS_OUTPUT input) : COLOR0
{
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);
    
	if (input.worldPosition.z < posicionNave.z && input.worldPosition.x > 90 && input.worldPosition.x < 127 && input.worldPosition.y > posicionNave.y)
        discard;
	
    return texelColor;

}


technique Borrado
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vertexShader();
		PixelShader = compile ps_3_0 pixelShader();
	}
}
//------------------------------------------Luzbelito-----------------------------------------

//Pixel Shader
float4 pixelLuzShader(VS_OUTPUT input) : COLOR0
{
    input.worldNormal = normalize(input.worldNormal);

    float3 lightDirection = normalize(posicionSol - input.worldPosition);
    float3 viewDirection = normalize(eyePosition - input.worldPosition);

	// Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Diffuse: N dot L
    float3 NdotL = dot(input.worldNormal, lightDirection);
    float3 diffuseLight = KDiffuse * diffuseColor * max(0.0, NdotL);

	//Componente Specular: (R dot V)^shininess
    float3 reflection = reflect(-lightDirection, input.worldNormal);
    float3 specularLight = pow(max(0.0, dot(viewDirection, reflection)), shininess) * KSpecular * specularColor;


    float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor + specularLight, texelColor.a);
    return finalColor;
	

}

technique Luzbelito
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vertexShader();
        PixelShader = compile ps_3_0 pixelLuzShader();
    }
}
//------------------------------------------NaveGameOver-----------------------------------------
//Pixel Shader
float4 pixelGameOverShader(VS_OUTPUT input) : COLOR0
{
    input.worldNormal = normalize(input.worldNormal);

    float3 lightDirection = normalize(posicionSol - input.worldPosition);
    float3 viewDirection = normalize(eyePosition - input.worldPosition);

	// Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Diffuse: N dot L
    float3 NdotL = dot(input.worldNormal, lightDirection);
    float3 diffuseLight = KDiffuse * diffuseColor * max(0.0, NdotL);

	//Componente Specular: (R dot V)^shininess
    float3 reflection = reflect(-lightDirection, input.worldNormal);
    float3 specularLight = pow(max(0.0, dot(viewDirection, reflection)), shininess) * KSpecular * specularColor;


    float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor + specularLight, texelColor.a);
    
    float4 gris = float4(0.6, 0.6, 0.6, 1);
    
    //if (tiempo > 1)
    //    tiempo = 1;
       
    float retorno = lerp(gris, finalColor, tiempo);
    
    return retorno;
	

}

technique NaveGameOver
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vertexShader();
        PixelShader = compile ps_3_0 pixelGameOverShader();
    }
}