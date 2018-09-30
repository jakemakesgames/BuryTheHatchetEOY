// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:8180,x:34188,y:32994,varname:node_8180,prsc:2|diff-6599-OUT,spec-9178-OUT,normal-2139-OUT;n:type:ShaderForge.SFN_VertexColor,id:3864,x:32602,y:33221,varname:node_3864,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:920,x:32651,y:32819,ptovrint:False,ptlb:Raised Floor,ptin:_RaisedFloor,varname:node_920,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:64734c9bc6df32149a0c9cb0b18693e1,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8279,x:32651,y:33036,ptovrint:False,ptlb:Bottom,ptin:_Bottom,varname:node_8279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7bbfb8818476e4641ba3e75f5225eb69,ntxv:2,isnm:False|UVIN-1966-OUT;n:type:ShaderForge.SFN_TexCoord,id:4766,x:32142,y:32997,varname:node_4766,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1966,x:32360,y:33209,varname:node_1966,prsc:2|A-4766-UVOUT,B-2413-OUT;n:type:ShaderForge.SFN_Vector1,id:2413,x:32013,y:33224,varname:node_2413,prsc:2,v1:8;n:type:ShaderForge.SFN_Tex2d,id:1520,x:32679,y:33628,ptovrint:False,ptlb:B_Normal,ptin:_B_Normal,varname:_B_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e00d5a9a0944134448432ccacf221b95,ntxv:3,isnm:True|UVIN-1966-OUT;n:type:ShaderForge.SFN_Tex2d,id:838,x:32679,y:33431,ptovrint:False,ptlb:A_Normal,ptin:_A_Normal,varname:_A_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:81b50d9cb6f3104448ec54c00a80101a,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:8673,x:33092,y:33405,varname:node_8673,prsc:2|A-838-RGB,B-1520-RGB,T-3864-G;n:type:ShaderForge.SFN_Normalize,id:2139,x:33286,y:33368,varname:node_2139,prsc:2|IN-8673-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:9178,x:33205,y:33198,varname:node_9178,prsc:2,a:0.1,b:0.7|IN-3864-G;n:type:ShaderForge.SFN_FragmentPosition,id:8700,x:33231,y:32037,varname:node_8700,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:4600,x:33485,y:32037,varname:node_4600,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-8700-X;n:type:ShaderForge.SFN_Add,id:6710,x:33684,y:32103,varname:node_6710,prsc:2|A-4600-OUT,B-7898-TSL;n:type:ShaderForge.SFN_Time,id:7898,x:33459,y:32210,varname:node_7898,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:4521,x:32798,y:32627,varname:node_4521,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-2297-UVOUT,TEX-7140-TEX;n:type:ShaderForge.SFN_Add,id:7061,x:33142,y:32281,varname:node_7061,prsc:2|A-8307-OUT;n:type:ShaderForge.SFN_Slider,id:1130,x:32237,y:32115,ptovrint:False,ptlb:Dissolve Amount,ptin:_DissolveAmount,varname:node_1130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:232,x:32206,y:33887,varname:node_232,prsc:2|A-6975-RGB,B-1897-RGB,T-5985-R;n:type:ShaderForge.SFN_ConstantLerp,id:2748,x:32261,y:34167,varname:node_2748,prsc:2,a:0.1,b:0.7|IN-5985-R;n:type:ShaderForge.SFN_Lerp,id:2114,x:32280,y:34384,varname:node_2114,prsc:2|A-4666-RGB,B-2451-RGB,T-5985-R;n:type:ShaderForge.SFN_Normalize,id:9269,x:32474,y:34347,varname:node_9269,prsc:2|IN-2114-OUT;n:type:ShaderForge.SFN_Tex2d,id:6975,x:31867,y:33820,ptovrint:False,ptlb:A_copy,ptin:_A_copy,varname:_A_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:64734c9bc6df32149a0c9cb0b18693e1,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1897,x:31867,y:34038,ptovrint:False,ptlb:B_copy,ptin:_B_copy,varname:_B_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7bbfb8818476e4641ba3e75f5225eb69,ntxv:2,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:5985,x:31867,y:34244,varname:node_5985,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:4666,x:31867,y:34410,ptovrint:False,ptlb:A_Normal_copy,ptin:_A_Normal_copy,varname:_A_Normal_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:81b50d9cb6f3104448ec54c00a80101a,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:2451,x:31867,y:34607,ptovrint:False,ptlb:B_Normal_copy,ptin:_B_Normal_copy,varname:_B_Normal_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e00d5a9a0944134448432ccacf221b95,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:637,x:31576,y:34210,varname:node_637,prsc:2|A-2027-UVOUT,B-8068-OUT;n:type:ShaderForge.SFN_TexCoord,id:2027,x:31358,y:33998,varname:node_2027,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:8068,x:31229,y:34225,varname:node_8068,prsc:2,v1:8;n:type:ShaderForge.SFN_OneMinus,id:8307,x:32612,y:32129,varname:node_8307,prsc:2|IN-1130-OUT;n:type:ShaderForge.SFN_Color,id:7644,x:32798,y:32477,ptovrint:False,ptlb:Sand Colour,ptin:_SandColour,varname:node_7644,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5588235,c2:0.3815416,c3:0,c4:1;n:type:ShaderForge.SFN_Panner,id:2297,x:32572,y:32396,varname:node_2297,prsc:2,spu:0.05,spv:0|UVIN-7160-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7160,x:32360,y:32396,varname:node_7160,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5020,x:33098,y:32611,varname:node_5020,prsc:2|A-7644-RGB,B-4521-R;n:type:ShaderForge.SFN_ValueProperty,id:1301,x:33245,y:32915,ptovrint:False,ptlb:sand intensity,ptin:_sandintensity,varname:node_1301,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.6;n:type:ShaderForge.SFN_Tex2dAsset,id:7140,x:32384,y:32665,ptovrint:False,ptlb:Sand Noise,ptin:_SandNoise,varname:node_7140,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False;n:type:ShaderForge.SFN_ChannelBlend,id:6599,x:33666,y:32998,varname:node_6599,prsc:2,chbt:1|M-3864-RGB,R-8279-RGB,G-920-RGB,B-6240-OUT,BTM-8279-RGB;n:type:ShaderForge.SFN_Add,id:9057,x:33290,y:32711,varname:node_9057,prsc:2|A-5020-OUT,B-920-RGB;n:type:ShaderForge.SFN_Lerp,id:6240,x:33448,y:32831,varname:node_6240,prsc:2|A-9057-OUT,B-920-RGB,T-1301-OUT;proporder:8279-920-1520-838-1130-7644-1301-7140;pass:END;sub:END;*/

Shader "Custom/FloorShader" {
    Properties {
        _Bottom ("Bottom", 2D) = "black" {}
        _RaisedFloor ("Raised Floor", 2D) = "black" {}
        _B_Normal ("B_Normal", 2D) = "bump" {}
        _A_Normal ("A_Normal", 2D) = "bump" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 1
        _SandColour ("Sand Colour", Color) = (0.5588235,0.3815416,0,1)
        _sandintensity ("sand intensity", Float ) = 0.6
        _SandNoise ("Sand Noise", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _RaisedFloor; uniform float4 _RaisedFloor_ST;
            uniform sampler2D _Bottom; uniform float4 _Bottom_ST;
            uniform sampler2D _B_Normal; uniform float4 _B_Normal_ST;
            uniform sampler2D _A_Normal; uniform float4 _A_Normal_ST;
            uniform float4 _SandColour;
            uniform float _sandintensity;
            uniform sampler2D _SandNoise; uniform float4 _SandNoise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _A_Normal_var = UnpackNormal(tex2D(_A_Normal,TRANSFORM_TEX(i.uv0, _A_Normal)));
                float2 node_1966 = (i.uv0*8.0);
                float3 _B_Normal_var = UnpackNormal(tex2D(_B_Normal,TRANSFORM_TEX(node_1966, _B_Normal)));
                float3 normalLocal = normalize(lerp(_A_Normal_var.rgb,_B_Normal_var.rgb,i.vertexColor.g));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float node_9178 = lerp(0.1,0.7,i.vertexColor.g);
                float3 specularColor = float3(node_9178,node_9178,node_9178);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Bottom_var = tex2D(_Bottom,TRANSFORM_TEX(node_1966, _Bottom));
                float4 _RaisedFloor_var = tex2D(_RaisedFloor,TRANSFORM_TEX(i.uv0, _RaisedFloor));
                float4 node_8030 = _Time;
                float2 node_2297 = (i.uv0+node_8030.g*float2(0.05,0));
                float4 node_4521 = tex2D(_SandNoise,TRANSFORM_TEX(node_2297, _SandNoise));
                float3 diffuseColor = (lerp( lerp( lerp( _Bottom_var.rgb, _Bottom_var.rgb, i.vertexColor.rgb.r ), _RaisedFloor_var.rgb, i.vertexColor.rgb.g ), lerp(((_SandColour.rgb*node_4521.r)+_RaisedFloor_var.rgb),_RaisedFloor_var.rgb,_sandintensity), i.vertexColor.rgb.b ));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _RaisedFloor; uniform float4 _RaisedFloor_ST;
            uniform sampler2D _Bottom; uniform float4 _Bottom_ST;
            uniform sampler2D _B_Normal; uniform float4 _B_Normal_ST;
            uniform sampler2D _A_Normal; uniform float4 _A_Normal_ST;
            uniform float4 _SandColour;
            uniform float _sandintensity;
            uniform sampler2D _SandNoise; uniform float4 _SandNoise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _A_Normal_var = UnpackNormal(tex2D(_A_Normal,TRANSFORM_TEX(i.uv0, _A_Normal)));
                float2 node_1966 = (i.uv0*8.0);
                float3 _B_Normal_var = UnpackNormal(tex2D(_B_Normal,TRANSFORM_TEX(node_1966, _B_Normal)));
                float3 normalLocal = normalize(lerp(_A_Normal_var.rgb,_B_Normal_var.rgb,i.vertexColor.g));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float node_9178 = lerp(0.1,0.7,i.vertexColor.g);
                float3 specularColor = float3(node_9178,node_9178,node_9178);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Bottom_var = tex2D(_Bottom,TRANSFORM_TEX(node_1966, _Bottom));
                float4 _RaisedFloor_var = tex2D(_RaisedFloor,TRANSFORM_TEX(i.uv0, _RaisedFloor));
                float4 node_2067 = _Time;
                float2 node_2297 = (i.uv0+node_2067.g*float2(0.05,0));
                float4 node_4521 = tex2D(_SandNoise,TRANSFORM_TEX(node_2297, _SandNoise));
                float3 diffuseColor = (lerp( lerp( lerp( _Bottom_var.rgb, _Bottom_var.rgb, i.vertexColor.rgb.r ), _RaisedFloor_var.rgb, i.vertexColor.rgb.g ), lerp(((_SandColour.rgb*node_4521.r)+_RaisedFloor_var.rgb),_RaisedFloor_var.rgb,_sandintensity), i.vertexColor.rgb.b ));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
