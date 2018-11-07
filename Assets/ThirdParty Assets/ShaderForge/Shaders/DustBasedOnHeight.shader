// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33409,y:32617,varname:node_3138,prsc:2|diff-7968-OUT,spec-3266-OUT,gloss-3266-OUT;n:type:ShaderForge.SFN_Tex2d,id:6277,x:32411,y:32178,ptovrint:False,ptlb:Dust Texture,ptin:_DustTexture,varname:node_6277,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9068f2bdd44df2448aa84778747f6fc6,ntxv:2,isnm:False|UVIN-4264-OUT;n:type:ShaderForge.SFN_Lerp,id:7968,x:33044,y:32590,varname:node_7968,prsc:2|A-7804-RGB,B-6792-OUT,T-3870-OUT;n:type:ShaderForge.SFN_Clamp01,id:3870,x:32566,y:32857,varname:node_3870,prsc:2|IN-3818-OUT;n:type:ShaderForge.SFN_Power,id:3818,x:32373,y:32858,varname:node_3818,prsc:2|VAL-6432-OUT,EXP-1690-OUT;n:type:ShaderForge.SFN_Multiply,id:6432,x:32160,y:33038,varname:node_6432,prsc:2|A-8630-OUT,B-7002-OUT;n:type:ShaderForge.SFN_Slider,id:1690,x:31719,y:33327,ptovrint:False,ptlb:contrast,ptin:_contrast,varname:node_1690,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.8205553,max:25;n:type:ShaderForge.SFN_Slider,id:7002,x:31719,y:33235,ptovrint:False,ptlb:Level,ptin:_Level,varname:node_7002,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2893796,max:10;n:type:ShaderForge.SFN_Dot,id:8630,x:31992,y:32858,varname:node_8630,prsc:2,dt:1|A-9032-OUT,B-5803-OUT;n:type:ShaderForge.SFN_Vector1,id:3972,x:31476,y:32547,varname:node_3972,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:9032,x:31715,y:32449,varname:node_9032,prsc:2|A-3972-OUT,B-6816-OUT,C-3972-OUT;n:type:ShaderForge.SFN_Slider,id:6816,x:31344,y:32651,ptovrint:False,ptlb:UpNode,ptin:_UpNode,varname:node_6816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Code,id:8095,x:32422,y:33163,varname:node_8095,prsc:2,code:ZAB3AGEAZAB3AAoAZAB3AGEACgA=,output:2,fname:Function_node_8095,width:247,height:132;n:type:ShaderForge.SFN_Color,id:7804,x:32434,y:32694,ptovrint:False,ptlb:Rock Base,ptin:_RockBase,varname:node_7804,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3088235,c2:0.1180796,c3:0.06358132,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:5511,x:31206,y:32872,varname:node_5511,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5803,x:31729,y:32952,varname:node_5803,prsc:2|A-150-OUT,B-7058-OUT;n:type:ShaderForge.SFN_Slider,id:7058,x:31203,y:33214,ptovrint:False,ptlb:world pos,ptin:_worldpos,varname:node_7058,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-100,cur:4,max:100;n:type:ShaderForge.SFN_ObjectPosition,id:620,x:31219,y:33037,varname:node_620,prsc:2;n:type:ShaderForge.SFN_Subtract,id:150,x:31520,y:32825,varname:node_150,prsc:2|A-5511-Y,B-620-Y;n:type:ShaderForge.SFN_TexCoord,id:8125,x:31839,y:31801,varname:node_8125,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4264,x:32093,y:31888,varname:node_4264,prsc:2|A-8125-UVOUT,B-6297-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6297,x:31839,y:32023,ptovrint:False,ptlb:texture tile,ptin:_texturetile,varname:node_6297,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Tex2d,id:2582,x:32049,y:32425,ptovrint:False,ptlb:node_2582,ptin:_node_2582,varname:node_2582,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-814-OUT;n:type:ShaderForge.SFN_Lerp,id:3827,x:32369,y:32393,varname:node_3827,prsc:2|A-6277-RGB,B-7804-RGB,T-8261-OUT;n:type:ShaderForge.SFN_TexCoord,id:4775,x:31775,y:32099,varname:node_4775,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:814,x:32029,y:32186,varname:node_814,prsc:2|A-4775-UVOUT,B-79-OUT;n:type:ShaderForge.SFN_ValueProperty,id:79,x:31775,y:32321,ptovrint:False,ptlb:noise tile,ptin:_noisetile,varname:_node_6297_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:8261,x:32197,y:32621,varname:node_8261,prsc:2|A-2582-R,B-8633-OUT;n:type:ShaderForge.SFN_Slider,id:8633,x:31858,y:32729,ptovrint:False,ptlb:dust noise opacity,ptin:_dustnoiseopacity,varname:node_8633,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:0,max:0;n:type:ShaderForge.SFN_Lerp,id:6792,x:32814,y:32339,varname:node_6792,prsc:2|A-3827-OUT,B-7804-RGB,T-3361-OUT;n:type:ShaderForge.SFN_Slider,id:3361,x:32499,y:32520,ptovrint:False,ptlb:overall opacity,ptin:_overallopacity,varname:node_3361,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:0,max:0;n:type:ShaderForge.SFN_Slider,id:3266,x:32891,y:32755,ptovrint:False,ptlb:Metallic + Roughness,ptin:_MetallicRoughness,varname:node_3266,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:3361-8633-1690-7002-7058-6816-7804-6297-79-6277-2582-3266;pass:END;sub:END;*/

Shader "Shader Forge/DustBasedOnHeight" {
    Properties {
        _overallopacity ("overall opacity", Range(1, 0)) = 0
        _dustnoiseopacity ("dust noise opacity", Range(1, 0)) = 0
        _contrast ("contrast", Range(0.1, 25)) = 0.8205553
        _Level ("Level", Range(0, 10)) = 0.2893796
        _worldpos ("world pos", Range(-100, 100)) = 4
        _UpNode ("UpNode", Range(0, 1)) = 1
        _RockBase ("Rock Base", Color) = (0.3088235,0.1180796,0.06358132,1)
        _texturetile ("texture tile", Float ) = 10
        _noisetile ("noise tile", Float ) = 0.1
        _DustTexture ("Dust Texture", 2D) = "black" {}
        _node_2582 ("node_2582", 2D) = "white" {}
        _MetallicRoughness ("Metallic + Roughness", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
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
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _DustTexture; uniform float4 _DustTexture_ST;
            uniform float _contrast;
            uniform float _Level;
            uniform float _UpNode;
            uniform float4 _RockBase;
            uniform float _worldpos;
            uniform float _texturetile;
            uniform sampler2D _node_2582; uniform float4 _node_2582_ST;
            uniform float _noisetile;
            uniform float _dustnoiseopacity;
            uniform float _overallopacity;
            uniform float _MetallicRoughness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _MetallicRoughness; // Convert roughness to gloss
                float perceptualRoughness = _MetallicRoughness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _MetallicRoughness;
                float specularMonochrome;
                float2 node_4264 = (i.uv0*_texturetile);
                float4 _DustTexture_var = tex2D(_DustTexture,TRANSFORM_TEX(node_4264, _DustTexture));
                float2 node_814 = (i.uv0*_noisetile);
                float4 _node_2582_var = tex2D(_node_2582,TRANSFORM_TEX(node_814, _node_2582));
                float node_3972 = 0.0;
                float3 diffuseColor = lerp(_RockBase.rgb,lerp(lerp(_DustTexture_var.rgb,_RockBase.rgb,(_node_2582_var.r*_dustnoiseopacity)),_RockBase.rgb,_overallopacity),saturate(pow((max(0,dot(float3(node_3972,_UpNode,node_3972),((i.posWorld.g-objPos.g)*_worldpos)))*_Level),_contrast))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor,1);
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
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _DustTexture; uniform float4 _DustTexture_ST;
            uniform float _contrast;
            uniform float _Level;
            uniform float _UpNode;
            uniform float4 _RockBase;
            uniform float _worldpos;
            uniform float _texturetile;
            uniform sampler2D _node_2582; uniform float4 _node_2582_ST;
            uniform float _noisetile;
            uniform float _dustnoiseopacity;
            uniform float _overallopacity;
            uniform float _MetallicRoughness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _MetallicRoughness; // Convert roughness to gloss
                float perceptualRoughness = _MetallicRoughness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _MetallicRoughness;
                float specularMonochrome;
                float2 node_4264 = (i.uv0*_texturetile);
                float4 _DustTexture_var = tex2D(_DustTexture,TRANSFORM_TEX(node_4264, _DustTexture));
                float2 node_814 = (i.uv0*_noisetile);
                float4 _node_2582_var = tex2D(_node_2582,TRANSFORM_TEX(node_814, _node_2582));
                float node_3972 = 0.0;
                float3 diffuseColor = lerp(_RockBase.rgb,lerp(lerp(_DustTexture_var.rgb,_RockBase.rgb,(_node_2582_var.r*_dustnoiseopacity)),_RockBase.rgb,_overallopacity),saturate(pow((max(0,dot(float3(node_3972,_UpNode,node_3972),((i.posWorld.g-objPos.g)*_worldpos)))*_Level),_contrast))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
