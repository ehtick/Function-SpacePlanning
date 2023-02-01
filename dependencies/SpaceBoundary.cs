using System.Collections.Generic;
using Elements.Geometry;
using System;
using Elements.Geometry.Solids;
using System.Linq;
using Newtonsoft.Json;
using LayoutFunctionCommon;
using SpacePlanning;

namespace Elements
{
    public partial class SpaceBoundary
    {

        public List<Line> AdjacentCorridorEdges { get; set; } = null;

        public Line AlignmentEdge { get; set; } = null;
        public double AvailableLength { get; set; } = 0;
        public Transform ToAlignmentEdge = null;
        public Transform FromAlignmentEdge = null;

        public Vector3? IndividualCentroid { get; set; } = null;
        public Vector3? ParentCentroid { get; set; } = null;

        public bool AutoPlaced { get; set; } = false;

        public int CountPlaced { get; set; } = 0;

        public int SpaceCount { get; set; } = 1;

        // Identity properties

        [JsonProperty("Level Add Id")]
        public string LevelAddId { get; set; }

        [JsonProperty("Relative Position")]
        public Vector3 RelativePosition { get; set; }

        // end identity properties

        [JsonIgnore]
        public LevelVolume LevelVolume { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public LevelElements LevelElements { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ProgramRequirement FulfilledProgramRequirement = null;

        // We need to maintain a pointer back to the original profile contained
        // in the level layout, so we can update its color and name.
        [Newtonsoft.Json.JsonIgnore]
        public Profile UnmodifiedProfile { get; set; }
        public static void SetRequirements(IEnumerable<ProgramRequirement> reqs)
        {
            Requirements = new Dictionary<string, ProgramRequirement>();
            foreach (var req in reqs)
            {
                if (Requirements.ContainsKey(req.QualifiedProgramName))
                {
                    Requirements[req.QualifiedProgramName].TotalArea += req.TotalArea;
                    Requirements[req.QualifiedProgramName].SpaceCount += req.SpaceCount;
                }
                else
                {
                    Requirements.Add(req.QualifiedProgramName, req);
                }
            }
            foreach (var kvp in Requirements)
            {
                var color = kvp.Value.Color;
                color.Alpha = 0.5;
                MaterialDict[kvp.Key] = new Material(kvp.Value.ProgramName, color, doubleSided: false);
            }
        }

        /// <summary>
        /// Static properties can persist across executions! need to reset to defaults w/ every execution.
        /// </summary>
        public static void Reset()
        {
            random = new Random(11);
            Requirements.Clear();
            MaterialDict = new Dictionary<string, Material>(materialDefaults);
        }

        public bool Match(ProgramAssignmentIdentity identity)
        {
            var lcs = this.LevelVolume?.LocalCoordinateSystem ?? new Transform();
            var levelMatch = identity.LevelAddId == this.LevelAddId;
            return levelMatch && this.Boundary.Contains(lcs.OfPoint(identity.RelativePosition));
        }

        public static bool TryGetRequirementsMatch(string nameToFind, out ProgramRequirement fullRequirement)
        {
            if (Requirements.TryGetValue(nameToFind ?? "unspecified", out fullRequirement))
            {
                return true;
            }
            else
            {
                var keyMatch = Requirements.Keys.FirstOrDefault(k => k.EndsWith($" - {nameToFind}"));
                if (keyMatch != null)
                {
                    fullRequirement = Requirements[keyMatch];
                    return true;
                }
            }
            return false;
        }
        public static Dictionary<string, ProgramRequirement> Requirements { get; private set; } = new Dictionary<string, ProgramRequirement>();

        private static Dictionary<string, Material> materialDefaults = new Dictionary<string, Material> {
            {"unspecified", new Material("Unspecified Space Type", new Color(0.8, 0.8, 0.8, 0.3), doubleSided: false)},
            {"Unassigned Space Type", new Material("Unspecified Space Type", new Color(0.8, 0.8, 0.8, 0.3), doubleSided: false)},
            {"unrecognized", new Material("Unspecified Space Type", new Color(0.8, 0.8, 0.2, 0.3), doubleSided: false)},
            {"Circulation", new Material("Circulation", new Color(0.996,0.965,0.863,0.5), doubleSided: false)}, //✅
            {"Open Office", new Material("Open Office", new Color(0.435,0.627,0.745,0.5), doubleSided: false)}, //✅  https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/35cb4053-4d39-47ef-9673-2dccdae1433b/SteelcaseOpenOffice-35cb4053-4d39-47ef-9673-2dccdae1433b.json
            {"Private Office", new Material("Private Office", new Color(0.122,0.271,0.361,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/69be76de-aaa1-4097-be0c-a97eb44d62e6/Private+Office-69be76de-aaa1-4097-be0c-a97eb44d62e6.json
            {"Lounge", new Material("Lounge", new Color(1.000,0.584,0.196,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/52df2dc8-3107-43c9-8a9f-e4b745baca1c/Steelcase-Lounge-52df2dc8-3107-43c9-8a9f-e4b745baca1c.json
            {"Classroom", new Material("Classroom", new Color(0.796,0.914,0.796,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/b23810e9-f565-4845-9b08-d6beb6223bea/Classroom-b23810e9-f565-4845-9b08-d6beb6223bea.json 
            {"Pantry", new Material("Pantry", new Color(0.5,0.714,0.745,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/599d1640-2584-42f7-8de1-e988267c360a/Pantry-599d1640-2584-42f7-8de1-e988267c360a.json
            {"Meeting Room", new Material("Meeting Room", new Color(0.380,0.816,0.608,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/251d637c-c570-43bd-ab33-f59f337506bb/Catalog-251d637c-c570-43bd-ab33-f59f337506bb.json
            {"Phone Booth", new Material("Phone Booth", new Color(0.976,0.788,0.129,0.5), doubleSided: false)},  //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/deacf056-2d7e-4396-8bdf-f30d581f2747/Phone+Booths-deacf056-2d7e-4396-8bdf-f30d581f2747.json
            {"Support", new Material("Support", new Color(0.447,0.498,0.573,0.5), doubleSided: false)},
            {"Reception", new Material("Reception", new Color(0.576,0.463,0.753,0.5), doubleSided: false)}, //✅ https://hypar-content-catalogs.s3-us-west-2.amazonaws.com/8762e4ec-7ddd-49b1-bcca-3f303f69f453/Reception-8762e4ec-7ddd-49b1-bcca-3f303f69f453.json 
            {"Open Collaboration", new Material("Open Collaboration", new Color(209.0/255, 224.0/255, 178.0/255, 0.5), doubleSided: false)},
            {"Data Hall", new Material("Data Hall", new Color(0.46,0.46,0.48,0.5), doubleSided: false)},
            {"Parking", new Material("Parking", new Color(0.447,0.498,0.573,0.5), doubleSided: false)}
        };

        internal void ComputeRelativePosition()
        {
            var lcs = this.LevelVolume?.LocalCoordinateSystem ?? new Transform();
            var internalPoint = this.Boundary.Perimeter.PointInternal();
            this.RelativePosition = lcs.Inverted().OfPoint(internalPoint);
        }

        public static Dictionary<string, Material> MaterialDict { get; private set; } = new Dictionary<string, Material>(materialDefaults);

        public string ProgramName
        {
            get
            {
                return ProgramType;
            }
            set
            {
                ProgramType = value;
            }
        }
        private static Random random = new Random(11);

        [JsonProperty("Room View")]
        public ViewScope RoomView { get; set; }

        public override void UpdateRepresentations()
        {
            var baseExtrude = new Extrude(Boundary.Transformed(new Transform(0, 0, 0.01)), Height, Vector3.ZAxis, false);
            var newS = new Solid();
            foreach (var face in baseExtrude.Solid.Faces)
            {
                newS.AddFace(face.Value.Outer.ToPolygon().Reversed(), face.Value.Inner?.Select(i => i.ToPolygon().Reversed())?.ToList());
            }
            // TODO - this bloats our JSON, since it has to serialize the whole solid! We should be able to create an "inside-out" solid.
            var cs = new ConstructedSolid(newS);
            Representation = new Representation(new[] { cs });
            var bbox = new BBox3(this);
            bbox = new BBox3(bbox.Min, bbox.Max - (0, 0, 0.1));
            RoomView = new ViewScope()
            {
                BoundingBox = bbox,
                Camera = new Camera((0, 0, -1), null, null),
                ClipWithBoundingBox = true
            };
            base.UpdateRepresentations();
        }
        public static SpaceBoundary Make(Profile profile, string fullyQualifiedName, Transform xform, double height, Vector3? parentCentroid = null, Vector3? individualCentroid = null, IEnumerable<Line> corridorSegments = null)
        {
            if (profile.Perimeter.IsClockWise())
            {
                profile = profile.Reversed();
            }
            MaterialDict.TryGetValue(fullyQualifiedName ?? "unspecified", out var material);
            var hasReqMatch = TryGetRequirementsMatch(fullyQualifiedName, out var fullReq);
            var name = hasReqMatch ? fullReq.HyparSpaceType : fullyQualifiedName;
            if (name == "unspecified")
            {
                name = "Unassigned Space Type";
            }
            var sb = new SpaceBoundary()
            {
                Boundary = profile,
                Cells = new List<Polygon> { profile.Perimeter },
                Area = profile.Area(),
                Height = height,
                Transform = xform,
                Material = material ?? MaterialDict["unrecognized"],
                Name = name
            };
            profile.Name = name;
            profile.AdditionalProperties["Color"] = sb.Material.Color;
            if (hasReqMatch)
            {
                fullReq.CountPlaced++;
                sb.FulfilledProgramRequirement = fullReq;
                sb.ProgramGroup = fullReq.ProgramGroup;
            }
            sb.ProgramName = fullyQualifiedName;
            sb.ParentCentroid = parentCentroid ?? xform.OfPoint(profile.Perimeter.Centroid());
            sb.IndividualCentroid = individualCentroid ?? xform.OfPoint(profile.Perimeter.Centroid());

            if (corridorSegments != null)
            {
                sb.AdjacentCorridorEdges = WallGeneration.FindAllEdgesAdjacentToSegments(profile.Perimeter.Segments(), corridorSegments, out var otherSegments);
            }
            return sb;
        }

        public void Remove()
        {
            if (this.LevelElements?.Elements != null && this.LevelElements.Elements.Contains(this))
            {
                this.LevelElements.Elements.Remove(this);
            }
            if (this.FulfilledProgramRequirement != null)
            {
                this.FulfilledProgramRequirement.CountPlaced--;
            }
        }
        public void SetProgram(string displayName)
        {
            if (!MaterialDict.TryGetValue(displayName ?? "unrecognized", out var material))
            {
                var color = random.NextColor();
                color.Alpha = 0.5;
                MaterialDict[displayName] = new Material(displayName, color, doubleSided: false);
                material = MaterialDict[displayName];
            }
            this.UnmodifiedProfile.AdditionalProperties["Color"] = material.Color;
            this.UnmodifiedProfile.Name = displayName;
            this.Material = material;
            this.ProgramName = displayName;
            if (this.FulfilledProgramRequirement != null)
            {
                this.FulfilledProgramRequirement.CountPlaced--;
            }
            var hasReqMatch = TryGetRequirementsMatch(displayName, out var fullReq);
            this.Name = hasReqMatch ? fullReq.HyparSpaceType : displayName;
            if (hasReqMatch)
            {
                fullReq.CountPlaced++;
                this.FulfilledProgramRequirement = fullReq;
            }
            this.ProgramType = displayName;
        }

        public void SetLevelProperties(LevelVolume volume)
        {
            this.AdditionalProperties["Building Name"] = volume.BuildingName;
            this.AdditionalProperties["Level Name"] = volume.Name;
            this.Level = volume.Id;
        }

        public SpaceBoundary Update(SpacePlanning.ProgramAssignmentOverride edit)
        {
            SetProgram(edit.Value.ProgramType ?? ProgramType);
            Height = edit.Value.Height ?? Height;
            return this;
        }

    }
}