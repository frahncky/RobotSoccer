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

export const WALL_HEIGHT_BOUNDS =
{
    (meter)      : [0.004, 0.012, 0.050],
    (centimeter) : 1.2,
    (millimeter) : 12,
    (inch)       : 0.4724
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

        annotation { "Name" : "Wall thickness" }
        isLength(definition.wallThickness, THICKNESS_BOUNDS);

        annotation { "Name" : "Wall height above base" }
        isLength(definition.wallHeight, WALL_HEIGHT_BOUNDS);
    }
    {
        const halfW = definition.chassisWidth / 2;
        const halfL = definition.chassisLength / 2;
        const halfOpening = definition.openingWidth / 2;
        const t = definition.wallThickness;

        // Base: U-shaped front opening to capture/control the ball.
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

        const baseBody = qCreatedBy(id + "baseExtrude", EntityType.BODY);

        // PLA perimeter reinforcement.
        // Rear wall + side walls + two front shoulders around the U opening.
        var wallSketch = newSketch(context, id + "wallSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skRectangle(wallSketch, "rearWall", {
                "firstCorner" : vector(-halfW, -halfL),
                "secondCorner" : vector(halfW, -halfL + t)
        });

        skRectangle(wallSketch, "leftWall", {
                "firstCorner" : vector(-halfW, -halfL),
                "secondCorner" : vector(-halfW + t, halfL)
        });

        skRectangle(wallSketch, "rightWall", {
                "firstCorner" : vector(halfW - t, -halfL),
                "secondCorner" : vector(halfW, halfL)
        });

        skRectangle(wallSketch, "frontLeftShoulder", {
                "firstCorner" : vector(-halfW, halfL - t),
                "secondCorner" : vector(-halfOpening, halfL)
        });

        skRectangle(wallSketch, "frontRightShoulder", {
                "firstCorner" : vector(halfOpening, halfL - t),
                "secondCorner" : vector(halfW, halfL)
        });

        skSolve(wallSketch);

        extrude(context, id + "wallExtrude", {
                "entities" : qSketchRegion(id + "wallSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.baseThickness + definition.wallHeight,
                "operationType" : NewBodyOperationType.ADD,
                "defaultScope" : false,
                "booleanScope" : baseBody
        });

        // Two longitudinal ribs improve stiffness for a printed PLA base
        // while keeping the center free for battery/electronics.
        const ribWidth = t;
        const ribOffset = definition.openingWidth / 2 + 12 * millimeter;

        var ribSketch = newSketch(context, id + "ribSketch", {
                "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
        });

        skRectangle(ribSketch, "leftRib", {
                "firstCorner" : vector(-ribOffset - ribWidth / 2, -halfL + 2 * t),
                "secondCorner" : vector(-ribOffset + ribWidth / 2, halfL - definition.openingDepth - 8 * millimeter)
        });

        skRectangle(ribSketch, "rightRib", {
                "firstCorner" : vector(ribOffset - ribWidth / 2, -halfL + 2 * t),
                "secondCorner" : vector(ribOffset + ribWidth / 2, halfL - definition.openingDepth - 8 * millimeter)
        });

        skSolve(ribSketch);

        extrude(context, id + "ribExtrude", {
                "entities" : qSketchRegion(id + "ribSketch"),
                "endBound" : BoundingType.BLIND,
                "depth" : definition.baseThickness + 5 * millimeter,
                "operationType" : NewBodyOperationType.ADD,
                "defaultScope" : false,
                "booleanScope" : qCreatedBy(id + "wallExtrude", EntityType.BODY)
        });

        setProperty(context, {
                "entities" : qCreatedBy(id + "ribExtrude", EntityType.BODY),
                "propertyType" : PropertyType.NAME,
                "value" : "RobotSoccer PLA chassis"
        });
    },
    {
        "chassisWidth" : 180 * millimeter,
        "chassisLength" : 180 * millimeter,
        "openingWidth" : 80 * millimeter,
        "openingDepth" : 35 * millimeter,
        "baseThickness" : 3 * millimeter,
        "wallThickness" : 3 * millimeter,
        "wallHeight" : 12 * millimeter
    });
