FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");

export const CHASSIS_DIAMETER_BOUNDS =
{
    (meter)      : [0.10, 0.18, 0.35],
    (centimeter) : 18,
    (millimeter) : 180,
    (inch)       : 7.0866
} as LengthBoundSpec;

export const OPENING_WIDTH_BOUNDS =
{
    (meter)      : [0.03, 0.08, 0.14],
    (centimeter) : 8,
    (millimeter) : 80,
    (inch)       : 3.1496
} as LengthBoundSpec;

export const OPENING_DEPTH_BOUNDS =
{
    (meter)      : [0.01, 0.03, 0.07],
    (centimeter) : 3,
    (millimeter) : 30,
    (inch)       : 1.1811
} as LengthBoundSpec;

export const THICKNESS_BOUNDS =
{
    (meter)      : [0.0015, 0.003, 0.008],
    (centimeter) : 0.3,
    (millimeter) : 3,
    (inch)       : 0.1181
} as LengthBoundSpec;

annotation { "Feature Type Name" : "01_RobotSoccer Chassis Hexagonal 3x120" }
export const robotSoccerChassis = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Hexagon tip-to-tip diameter" }
        isLength(definition.diameter, CHASSIS_DIAMETER_BOUNDS);

        annotation { "Name" : "Front opening width" }
        isLength(definition.openingWidth, OPENING_WIDTH_BOUNDS);

        annotation { "Name" : "Front opening depth" }
        isLength(definition.openingDepth, OPENING_DEPTH_BOUNDS);

        annotation { "Name" : "Base thickness" }
        isLength(definition.baseThickness, THICKNESS_BOUNDS);
    }
    {
        const r = definition.diameter / 2;
        const h = r * sqrt(3) / 2;
        const halfOpening = definition.openingWidth / 2;

        // Regular hexagon with a front central notch for ball entry.
        var s = newSketch(context, id + "hexSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skPolyline(s, "hexWithOpening", {
                "points" : [
                    vector(-r, 0 * millimeter),
                    vector(-r / 2, -h),
                    vector( r / 2, -h),
                    vector( r, 0 * millimeter),
                    vector( r / 2, h),
                    vector( halfOpening, h),
                    vector( halfOpening, h - definition.openingDepth),
                    vector(-halfOpening, h - definition.openingDepth),
                    vector(-halfOpening, h),
                    vector(-r / 2, h),
                    vector(-r, 0 * millimeter)
                ]
        });

        skSolve(s);

        extrude(context, id + "baseExtrude", {
                "entities" : qSketchRegion(id + "hexSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.baseThickness
        });
    },
    {
        "diameter" : 180 * millimeter,
        "openingWidth" : 80 * millimeter,
        "openingDepth" : 30 * millimeter,
        "baseThickness" : 3 * millimeter
    });