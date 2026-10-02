FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");

export const MOTOR_WIDTH_BOUNDS =
{
    (meter)      : [0.008, 0.012, 0.025],
    (centimeter) : 1.2,
    (millimeter) : 12,
    (inch)       : 0.4724
} as LengthBoundSpec;

export const HOLDER_LENGTH_BOUNDS =
{
    (meter)      : [0.012, 0.022, 0.060],
    (centimeter) : 2.2,
    (millimeter) : 22,
    (inch)       : 0.8661
} as LengthBoundSpec;

export const WALL_BOUNDS =
{
    (meter)      : [0.0015, 0.0025, 0.006],
    (centimeter) : 0.25,
    (millimeter) : 2.5,
    (inch)       : 0.0984
} as LengthBoundSpec;

export const CLEARANCE_BOUNDS =
{
    (meter)      : [0.0002, 0.0004, 0.0015],
    (centimeter) : 0.04,
    (millimeter) : 0.4,
    (inch)       : 0.0157
} as LengthBoundSpec;

export const HOLDER_HEIGHT_BOUNDS =
{
    (meter)      : [0.006, 0.013, 0.030],
    (centimeter) : 1.3,
    (millimeter) : 13,
    (inch)       : 0.5118
} as LengthBoundSpec;

annotation { "Feature Type Name" : "02_RobotSoccer N20 Motor Holder" }
export const robotSoccerN20Holder = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Motor body width" }
        isLength(definition.motorWidth, MOTOR_WIDTH_BOUNDS);

        annotation { "Name" : "Holder length" }
        isLength(definition.holderLength, HOLDER_LENGTH_BOUNDS);

        annotation { "Name" : "PLA wall thickness" }
        isLength(definition.wallThickness, WALL_BOUNDS);

        annotation { "Name" : "Fit clearance" }
        isLength(definition.clearance, CLEARANCE_BOUNDS);

        annotation { "Name" : "Holder height" }
        isLength(definition.holderHeight, HOLDER_HEIGHT_BOUNDS);
    }
    {
        const innerWidth = definition.motorWidth + 2 * definition.clearance;
        const outerWidth = innerWidth + 2 * definition.wallThickness;
        const halfOuter = outerWidth / 2;
        const halfInner = innerWidth / 2;
        const halfLength = definition.holderLength / 2;
        const baseThickness = 3 * millimeter;

        var holderSketch = newSketch(context, id + "holderSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        // Single closed U-shaped profile in top view:
        // full base strip plus two side rails, all as one connected region.
        skPolyline(holderSketch, "uProfile", {
                "points" : [
                    vector(-halfOuter, -halfLength),
                    vector( halfOuter, -halfLength),
                    vector( halfOuter,  halfLength),
                    vector( halfInner,  halfLength),
                    vector( halfInner, -halfLength + baseThickness),
                    vector(-halfInner, -halfLength + baseThickness),
                    vector(-halfInner,  halfLength),
                    vector(-halfOuter,  halfLength),
                    vector(-halfOuter, -halfLength)
                ]
        });

        skSolve(holderSketch);

        extrude(context, id + "holderExtrude", {
                "entities" : qSketchRegion(id + "holderSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.holderHeight
        });
    },
    {
        "motorWidth" : 12 * millimeter,
        "holderLength" : 22 * millimeter,
        "wallThickness" : 2.5 * millimeter,
        "clearance" : 0.4 * millimeter,
        "holderHeight" : 13 * millimeter
    });
