FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");

export const MOTOR_WIDTH_BOUNDS =
{
    (meter)      : [0.008, 0.012, 0.025],
    (centimeter) : 1.2,
    (millimeter) : 12,
    (inch)       : 0.4724
} as LengthBoundSpec;

export const MOTOR_HEIGHT_BOUNDS =
{
    (meter)      : [0.008, 0.010, 0.025],
    (centimeter) : 1.0,
    (millimeter) : 10,
    (inch)       : 0.3937
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

annotation { "Feature Type Name" : "RobotSoccer N20 Motor Holder" }
export const robotSoccerN20Holder = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Motor body width" }
        isLength(definition.motorWidth, MOTOR_WIDTH_BOUNDS);

        annotation { "Name" : "Motor body height" }
        isLength(definition.motorHeight, MOTOR_HEIGHT_BOUNDS);

        annotation { "Name" : "Holder length" }
        isLength(definition.holderLength, HOLDER_LENGTH_BOUNDS);

        annotation { "Name" : "PLA wall thickness" }
        isLength(definition.wallThickness, WALL_BOUNDS);

        annotation { "Name" : "Fit clearance" }
        isLength(definition.clearance, CLEARANCE_BOUNDS);
    }
    {
        const innerWidth = definition.motorWidth + 2 * definition.clearance;
        const outerWidth = innerWidth + 2 * definition.wallThickness;
        const halfOuter = outerWidth / 2;
        const halfInner = innerWidth / 2;
        const halfLength = definition.holderLength / 2;

        // Base
        var baseSketch = newSketch(context, id + "baseSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skRectangle(baseSketch, "base", {
                "firstCorner" : vector(-halfOuter, -halfLength),
                "secondCorner" : vector(halfOuter, halfLength)
        });

        skSolve(baseSketch);

        extrude(context, id + "baseExtrude", {
                "entities" : qSketchRegion(id + "baseSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : 3 * millimeter
        });

        // Left wall as independent body
        var leftSketch = newSketch(context, id + "leftSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skRectangle(leftSketch, "leftRail", {
                "firstCorner" : vector(-halfOuter, -halfLength),
                "secondCorner" : vector(-halfInner, halfLength)
        });

        skSolve(leftSketch);

        extrude(context, id + "leftExtrude", {
                "entities" : qSketchRegion(id + "leftSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.motorHeight + definition.wallThickness
        });

        // Right wall as independent body
        var rightSketch = newSketch(context, id + "rightSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skRectangle(rightSketch, "rightRail", {
                "firstCorner" : vector(halfInner, -halfLength),
                "secondCorner" : vector(halfOuter, halfLength)
        });

        skSolve(rightSketch);

        extrude(context, id + "rightExtrude", {
                "entities" : qSketchRegion(id + "rightSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.motorHeight + definition.wallThickness
        });
    },
    {
        "motorWidth" : 12 * millimeter,
        "motorHeight" : 10 * millimeter,
        "holderLength" : 22 * millimeter,
        "wallThickness" : 2.5 * millimeter,
        "clearance" : 0.4 * millimeter
    });
