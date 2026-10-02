FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");

export const CHASSIS_SIZE_BOUNDS =
{
    (meter)      : [0.08, 0.18, 0.50],
    (centimeter) : 18,
    (millimeter) : 180,
    (inch)       : 7.0866
} as LengthBoundSpec;

export const OPENING_WIDTH_BOUNDS =
{
    (meter)      : [0.03, 0.08, 0.20],
    (centimeter) : 8,
    (millimeter) : 80,
    (inch)       : 3.1496
} as LengthBoundSpec;

export const OPENING_DEPTH_BOUNDS =
{
    (meter)      : [0.01, 0.035, 0.10],
    (centimeter) : 3.5,
    (millimeter) : 35,
    (inch)       : 1.378
} as LengthBoundSpec;

export const THICKNESS_BOUNDS =
{
    (meter)      : [0.0015, 0.003, 0.012],
    (centimeter) : 0.3,
    (millimeter) : 3,
    (inch)       : 0.1181
} as LengthBoundSpec;

annotation { "Feature Type Name" : "01_RobotSoccer Chassis PLA" }
export const robotSoccerChassis = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Chassis width" }
        isLength(definition.chassisWidth, CHASSIS_SIZE_BOUNDS);

        annotation { "Name" : "Chassis length" }
        isLength(definition.chassisLength, CHASSIS_SIZE_BOUNDS);

        annotation { "Name" : "Front opening width" }
        isLength(definition.openingWidth, OPENING_WIDTH_BOUNDS);

        annotation { "Name" : "Front opening depth" }
        isLength(definition.openingDepth, OPENING_DEPTH_BOUNDS);

        annotation { "Name" : "Base thickness" }
        isLength(definition.baseThickness, THICKNESS_BOUNDS);
    }
    {
        const halfW = definition.chassisWidth / 2;
        const halfL = definition.chassisLength / 2;
        const halfOpening = definition.openingWidth / 2;

        var chassisSketch = newSketch(context, id + "chassisSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skPolyline(chassisSketch, "chassisOutline", {
                "points" : [
                    vector(-halfW, -halfL),
                    vector( halfW, -halfL),
                    vector( halfW,  halfL),
                    vector( halfOpening, halfL),
                    vector( halfOpening, halfL - definition.openingDepth),
                    vector(-halfOpening, halfL - definition.openingDepth),
                    vector(-halfOpening, halfL),
                    vector(-halfW, halfL),
                    vector(-halfW, -halfL)
                ]
        });

        skSolve(chassisSketch);

        extrude(context, id + "baseExtrude", {
                "entities" : qSketchRegion(id + "chassisSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.baseThickness
        });
    },
    {
        "chassisWidth" : 180 * millimeter,
        "chassisLength" : 180 * millimeter,
        "openingWidth" : 80 * millimeter,
        "openingDepth" : 35 * millimeter,
        "baseThickness" : 3 * millimeter
    });