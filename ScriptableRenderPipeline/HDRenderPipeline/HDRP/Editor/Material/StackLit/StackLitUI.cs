using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    class StackLitGUI : BaseMaterialGUI
    {
        protected static class StylesStackLit
        {
            public static GUIContent useLocalPlanarMapping = new GUIContent("Use Local Planar Mapping", "Use local space for planar/triplanar mapping instead of world space");
        };

        #region Strings

        protected const string k_DoubleSidedNormalMode = "_DoubleSidedNormalMode";

        protected const string k_UVBase = "_UVBase";

        // Base
        protected const string k_BaseColor = "_BaseColor";
        protected const string k_BaseColorMap = "_BaseColorMap";
        protected const string k_BaseColorMapUV = "_BaseColorMapUV";

        protected const string k_Metallic = "_Metallic";
        protected const string k_MetallicMap = "_MetallicMap";
        protected const string k_MetallicMapUV = "_MetallicMapUV";

        protected const string k_Smoothness1 = "_SmoothnessA";
        protected const string k_Smoothness1Map = "_SmoothnessAMap";
        protected const string k_Smoothness1MapUV = "_SmoothnessAMapUV";

        protected const string k_NormalMap = "_NormalMap";
        protected const string k_NormalMapUV = "_NormalMapUV";
        protected const string k_NormalScale = "_NormalScale";

        protected const string k_AmbientOcclusion = "_AmbientOcclusion";
        protected const string k_AmbientOcclusionMap = "_AmbientOcclusionMap";
        protected const string k_AmbientOcclusionMapUV = "_AmbientOcclusionMapUV";

        // Emissive
        protected const string k_EmissiveColor = "_EmissiveColor";
        protected const string k_EmissiveColorMap = "_EmissiveColorMap";
        protected const string k_EmissiveColorMapUV = "_EmissiveColorMapUV";
        protected const string k_EmissiveIntensity = "_EmissiveIntensity";
        protected const string k_AlbedoAffectEmissive = "_AlbedoAffectEmissive";

        // Coat
        protected const string k_CoatEnable = "_CoatEnable";
        protected const string k_CoatSmoothness = "_CoatSmoothness";
        protected const string k_CoatIor = "_CoatIor";
        protected const string k_CoatThickness = "_CoatThickness";
        protected const string k_CoatExtinction = "_CoatExtinction";

        // SSS
        protected const string k_DiffusionProfile = "_DiffusionProfile";
        protected const string k_SubsurfaceMask = "_SubsurfaceMask";
        protected const string k_SubsurfaceMaskMap = "_SubsurfaceMaskMap";
        protected const string k_SubsurfaceMaskMapUV = "_SubsurfaceMaskMapUV";

        // Translucency
        protected const string k_Thickness = "_Thickness";
        protected const string k_ThicknessMap = "_ThicknessMap";
        protected const string k_ThicknessMapUV = "_ThicknessMapUV";

        // Second Lobe.
        protected const string k_Smoothness2 = "_SmoothnessB";
        protected const string k_Smoothness2Map = "_SmoothnessBMap";
        protected const string k_Smoothness2MapUV = "_SmoothnessBMapUV";

        protected const string k_LobeMix = "_LobeMix";

        // Anisotropy
        protected const string k_Anisotropy = "_Anisotropy";

        //// transparency params
        //protected MaterialProperty transmissionEnable = null;
        //protected const string kTransmissionEnable = "_TransmissionEnable";

        //protected MaterialProperty ior = null;
        //protected const string kIor = "_Ior";
        //protected MaterialProperty transmittanceColor = null;
        //protected const string kTransmittanceColor = "_TransmittanceColor";
        //protected MaterialProperty transmittanceColorMap = null;
        //protected const string kTransmittanceColorMap = "_TransmittanceColorMap";
        //protected MaterialProperty atDistance = null;
        //protected const string kATDistance = "_ATDistance";
        //protected MaterialProperty thicknessMultiplier = null;
        //protected const string kThicknessMultiplier = "_ThicknessMultiplier";
        //protected MaterialProperty refractionModel = null;
        //protected const string kRefractionModel = "_RefractionModel";
        //protected MaterialProperty refractionSSRayModel = null;
        //protected const string kRefractionSSRayModel = "_RefractionSSRayModel";
        #endregion

        // Add the properties into an array.
        private readonly GroupProperty _baseMaterialProperties = null;
        private readonly GroupProperty _materialProperties = null;

        public StackLitGUI()
        {
            _baseMaterialProperties = new GroupProperty(this, "_BaseMaterial", new BaseProperty[]
            {
                // JFFTODO: Find the proper condition, and proper way to display this.
                new Property(this, k_DoubleSidedNormalMode, "Normal mode", "This will modify the normal base on the selected mode. Mirror: Mirror the normal with vertex normal plane, Flip: Flip the normal.", false),
            });

            _materialProperties = new GroupProperty(this, "_Material", new BaseProperty[]
            {
                new GroupProperty(this, "_Standard", "Standard", new BaseProperty[]
                {
                    new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Base Color + Opacity", "Albedo (RGB) and Opacity (A)", true, false),
                    new TextureProperty(this, k_MetallicMap, k_Metallic, "Metallic", "Metallic", false, false),
                    new TextureProperty(this, k_Smoothness1Map, k_Smoothness1, "Smoothness", "Smoothness", false, false),
                    // TODO: Special case for normal maps.
                    new TextureProperty(this, k_NormalMap, k_NormalScale, "Normal", "Normal Map", false, false, true),

                    //new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Dielectric IoR", "Index of Refraction for Dielectric", false),

                    new TextureProperty(this, k_AmbientOcclusionMap, k_AmbientOcclusion, "AmbientOcclusion", "AmbientOcclusion Map", false, false),
                }),

                new GroupProperty(this, "_Emissive", "Emissive", new BaseProperty[]
                {
                    new TextureProperty(this, k_EmissiveColorMap, k_EmissiveColor, "Emissive Color", "Emissive", true, false),
                    new Property(this, k_EmissiveIntensity, "Emissive Intensity", "Emissive", false),
                    new Property(this, k_AlbedoAffectEmissive, "Albedo Affect Emissive", "Specifies whether or not the emissive color is multiplied by the albedo.", false),
                }),

                new GroupProperty(this, "_Coat", "Coat", new BaseProperty[]
                {
                    new Property(this, "_CoatEnable", "Coat Enable", "Enable coat layer with true vertical physically based BSDF mixing", false),
                    new Property(this, "_CoatSmoothness", "Coat Smoothness", "Top layer smoothness", false),
                    new Property(this, "_CoatIor", "Coat IOR", "Index of refraction", false),
                    new Property(this, "_CoatThickness", "Coat Thickness", "Coat thickness", false),
                    new Property(this, "_CoatExtinction", "Coat Absorption", "Coat absorption tint (the thicker the coat, the more that color is removed)", false),
                }),

                new GroupProperty(this, "_SSS", "Sub-Surface Scattering", new BaseProperty[]
                {
                    new DiffusionProfileProperty(this, k_DiffusionProfile, "Diffusion Profile", "A profile determines the shape of the SSS/transmission filter.", false),
                    new TextureProperty(this, k_SubsurfaceMaskMap, k_SubsurfaceMask, "Subsurface mask map (R)", "Determines the strength of the subsurface scattering effect.", false, false),
                }/*, _ => _materialId == MaterialId.SubSurfaceScattering*/),

                new GroupProperty(this, "_Lobe2", "Second Specular Lobe", new BaseProperty[]
                {
                    new TextureProperty(this, k_Smoothness2Map, k_Smoothness2, "Smoothness2", "Smoothness2", false, false),
                    new Property(this, k_LobeMix, "Lobe Mix", "Lobe Mix", false),
                }),

                new GroupProperty(this, "_Anisotropy", "Anisotropy", new BaseProperty[]
                {
                    new Property(this, k_Anisotropy, "Anisotropy", "Anisotropy of base layer", false),
                }),

                new GroupProperty(this, "_Transmission", "Transmission", new BaseProperty[]
                {
                    new DiffusionProfileProperty(this, k_DiffusionProfile, "Diffusion Profile", "A profile determines the shape of the SSS/transmission filter.", false),
                    new TextureProperty(this, k_ThicknessMap, k_Thickness, "Thickness", "If subsurface scattering is enabled, low values allow some light to be transmitted through the object.", false),
                }),

                //new GroupProperty(this, "_Iridescence", "Iridescence", new BaseProperty[]
                //{
                //    new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Index of Refraction", "Index of Refraction for Iridescence", false),
                //    new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Thickness", "Thickness", false),
                //}),

                //new GroupProperty(this, "_Glint", "Glint", new BaseProperty[]
                //{
                //    new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Density", "Density:", false),
                //    new TextureProperty(this, k_BaseColorMap, k_BaseColor, "Tint", "Tint", false),
                //}),
            });
        }

        protected override bool ShouldEmissionBeEnabled(Material material)
        {
            return material.GetFloat(k_EmissiveIntensity) > 0.0f;
        }

        protected override void FindBaseMaterialProperties(MaterialProperty[] props)
        {
            base.FindBaseMaterialProperties(props);
            _baseMaterialProperties.OnFindProperty(props);
        }

        protected override void FindMaterialProperties(MaterialProperty[] props)
        {
            //base.FindMaterialProperties(props);
            _materialProperties.OnFindProperty(props);
        }

        protected override void BaseMaterialPropertiesGUI()
        {
            base.BaseMaterialPropertiesGUI();
            _baseMaterialProperties.OnGUI();
        }

        protected override void MaterialPropertiesGUI(Material material)
        {
            //if (GUILayout.Button("Generate All Properties"))
            //{
            //    Debug.Log(_materialProperties.ToShaderPropertiesStringInternal());
            //}

            _materialProperties.OnGUI();
        }

        protected override void MaterialPropertiesAdvanceGUI(Material material)
        {
        }

        protected override void VertexAnimationPropertiesGUI()
        {
        }

        protected override void SetupMaterialKeywordsAndPassInternal(Material material)
        {
            SetupMaterialKeywordsAndPass(material);
        }

        protected static void SetupTextureMaterialProperty(Material material, string basePropertyName)
        {
            // TODO: Caution this can generate a lot of garbage collection call ?
            string useMapPropertyName = basePropertyName + "UseMap";
            string mapPropertyName = basePropertyName + "Map";
            string remapPropertyName = basePropertyName + "Remap";
            string invertPropertyName = basePropertyName + "RemapInverted";
            string rangePropertyName = basePropertyName + "Range";
            string channelPropertyName = basePropertyName + "MapChannel";
            string channelMaskPropertyName = basePropertyName + "MapChannelMask";

            if (material.GetTexture(mapPropertyName))
            {
                Vector4 rangeVector = material.GetVector(remapPropertyName);
                if (material.HasProperty(invertPropertyName) && material.GetFloat(invertPropertyName) > 0.0f)
                {
                    float s = rangeVector.x;
                    rangeVector.x = rangeVector.y;
                    rangeVector.y = s;
                }

                material.SetFloat(useMapPropertyName, 1.0f);
                material.SetVector(rangePropertyName, rangeVector);

                int channel = (int)material.GetFloat(channelPropertyName);
                switch (channel)
                {
                case 0:
                    material.SetVector(channelMaskPropertyName, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
                    break;
                case 1:
                    material.SetVector(channelMaskPropertyName, new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
                    break;
                case 2:
                    material.SetVector(channelMaskPropertyName, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
                    break;
                case 3:
                    material.SetVector(channelMaskPropertyName, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
                    break;
                }
            }
            else
            {
                material.SetFloat(useMapPropertyName, 0.0f);
                material.SetVector(rangePropertyName, new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
                material.SetVector(channelMaskPropertyName, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
            }
        }

        // All Setup Keyword functions must be static. It allow to create script to automatically update the shaders with a script if code change
        public static void SetupMaterialKeywordsAndPass(Material material)
        {
            //TODO see BaseLitUI.cs:SetupBaseLitKeywords (stencil etc)
            SetupBaseUnlitKeywords(material);
            SetupBaseUnlitMaterialPass(material);

            bool doubleSidedEnable = material.GetFloat(kDoubleSidedEnable) > 0.0f;

            if (doubleSidedEnable)
            {
                BaseLitGUI.DoubleSidedNormalMode doubleSidedNormalMode = (BaseLitGUI.DoubleSidedNormalMode)material.GetFloat(k_DoubleSidedNormalMode);
                switch (doubleSidedNormalMode)
                {
                    case BaseLitGUI.DoubleSidedNormalMode.Mirror: // Mirror mode (in tangent space)
                        material.SetVector("_DoubleSidedConstants", new Vector4(1.0f, 1.0f, -1.0f, 0.0f));
                        break;

                    case BaseLitGUI.DoubleSidedNormalMode.Flip: // Flip mode (in tangent space)
                        material.SetVector("_DoubleSidedConstants", new Vector4(-1.0f, -1.0f, -1.0f, 0.0f));
                        break;

                    case BaseLitGUI.DoubleSidedNormalMode.None: // None mode (in tangent space)
                        material.SetVector("_DoubleSidedConstants", new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
                        break;
                }
            }

            //NOTE: For SSS in forward and split lighting, obviously we don't have a gbuffer pass,
            // so no stencil tagging there, but velocity? To check...

            //TODO: stencil state, displacement, wind, depthoffset, tessellation

            SetupMainTexForAlphaTestGI("_BaseColorMap", "_BaseColor", material);

            //TODO: disable DBUFFER

            CoreUtils.SetKeyword(material, "_NORMALMAP_TANGENT_SPACE", true);
            CoreUtils.SetKeyword(material, "_NORMALMAP", material.GetTexture(k_NormalMap));

            SetupTextureMaterialProperty(material, k_Metallic);
            SetupTextureMaterialProperty(material, k_Smoothness1);
            SetupTextureMaterialProperty(material, k_Smoothness2);
            SetupTextureMaterialProperty(material, k_AmbientOcclusion);
            SetupTextureMaterialProperty(material, k_SubsurfaceMask);
            SetupTextureMaterialProperty(material, k_Thickness);

            // Check if we are using specific UVs.
            TextureProperty.UVMapping[] uvIndices = new[]
            {
                (TextureProperty.UVMapping)material.GetFloat(k_BaseColorMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_MetallicMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_NormalMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_Smoothness1MapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_Smoothness2MapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_AmbientOcclusionMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_EmissiveColorMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_SubsurfaceMaskMapUV),
                (TextureProperty.UVMapping)material.GetFloat(k_ThicknessMapUV),
            };

            bool requireUv2 = false;
            bool requireUv3 = false;
            bool requireTriplanar = false;
            for (int i = 0; i < uvIndices.Length; ++i)
            {
                requireUv2 = requireUv2 || uvIndices[i] == TextureProperty.UVMapping.UV2;
                requireUv3 = requireUv3 || uvIndices[i] == TextureProperty.UVMapping.UV3;
                requireTriplanar = requireTriplanar || uvIndices[i] == TextureProperty.UVMapping.Triplanar;
            }

            //CoreUtils.SetKeyword(material, "_USE_UV2", requireUv2);
            //CoreUtils.SetKeyword(material, "_USE_UV3", requireUv3);
            CoreUtils.SetKeyword(material, "_USE_TRIPLANAR", requireTriplanar);

            bool anisotropyEnabled = material.HasProperty(k_Anisotropy) && (material.GetFloat(k_Anisotropy) != 0.0f);
            // TODO: When we have a map, also test for map for enable. (This scheme doesn't allow enabling from
            // neutral value though, better to still have flag and uncheck it in UI code when reach neutral
            // value and re-enable otherwise).
            bool coatEnabled = material.HasProperty(k_CoatEnable) && (material.GetFloat(k_CoatEnable) > 0.0f);
            bool dualLobeEnabled = material.HasProperty(k_LobeMix) && (material.GetFloat(k_LobeMix) > 0.0f);

            // Note that we don't use the materialId (cf Lit.shader) mechanism in the UI
            CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_ANISOTROPY", anisotropyEnabled);
            CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_COAT", coatEnabled);
            CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_DUAL_LOBE", dualLobeEnabled);

        }
    }
} // namespace UnityEditor
