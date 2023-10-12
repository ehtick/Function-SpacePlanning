//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.1.21.0 (Newtonsoft.Json v13.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------
using Elements;
using Elements.GeoJSON;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Spatial;
using Elements.Validators;
using Elements.Serialization.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Line = Elements.Geometry.Line;
using Polygon = Elements.Geometry.Polygon;

namespace Elements
{
    #pragma warning disable // Disable all warnings

    /// <summary>Represents camera settings</summary>
    [JsonConverter(typeof(Elements.Serialization.JSON.JsonInheritanceConverter), "discriminator")]
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.21.0 (Newtonsoft.Json v13.0.0.0)")]
    public partial class Camera 
    {
        [JsonConstructor]
        public Camera(Vector3 @angle, CameraNamedPosition? @namedPosition, CameraProjection? @projection)
        {
            this.Angle = @angle;
            this.NamedPosition = @namedPosition;
            this.Projection = @projection;
            }
        
        // Empty constructor
        public Camera()
        {
        }
    
        /// <summary>A unit vector in model coordinates indicating which direction the camera is pointing.</summary>
        [JsonProperty("angle", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Vector3 Angle { get; set; }
    
        /// <summary>Camera positions, viewing from this direction to the opposite direction. Do not set angle if setting this.</summary>
        [JsonProperty("named_position", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CameraNamedPosition? NamedPosition { get; set; }
    
        /// <summary>How the camera collapses the 3d scene into a 2d image</summary>
        [JsonProperty("projection", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CameraProjection? Projection { get; set; }
    
    
    }
}