FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");

// ===== 01_RobotSoccerChassis.fs =====
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

// ===== 02_RobotSoccerN20MotorHolder.fs =====
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

// ===== 03_RobotSoccerBatteryTray.fs =====
export const TRAY_W = {(millimeter):[25,45,80]} as LengthBoundSpec;
export const TRAY_L = {(millimeter):[40,75,120]} as LengthBoundSpec;
export const TRAY_T = {(millimeter):[2,3,6]} as LengthBoundSpec;
annotation { "Feature Type Name" : "03_RobotSoccer Battery Tray" }
export const robotSoccerBatteryTray = defineFeature(function(context is Context,id is Id,definition is map)
precondition {
 annotation{"Name":"Width"} isLength(definition.w,TRAY_W);
 annotation{"Name":"Length"} isLength(definition.l,TRAY_L);
 annotation{"Name":"Thickness"} isLength(definition.t,TRAY_T);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.t});
},{"w":45*millimeter,"l":75*millimeter,"t":3*millimeter});

// ===== 04_RobotSoccerElectronicsDeck.fs =====
export const DECK_W={(millimeter):[60,120,170]} as LengthBoundSpec;
export const DECK_L={(millimeter):[60,120,170]} as LengthBoundSpec;
export const DECK_T={(millimeter):[2,3,6]} as LengthBoundSpec;
annotation { "Feature Type Name" : "04_RobotSoccer Electronics Deck" }
export const robotSoccerElectronicsDeck=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Width"} isLength(definition.w,DECK_W);
 annotation{"Name":"Length"} isLength(definition.l,DECK_L);
 annotation{"Name":"Thickness"} isLength(definition.t,DECK_T);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.t});
},{"w":120*millimeter,"l":120*millimeter,"t":3*millimeter});

// ===== 05_RobotSoccerKickerArm.fs =====
export const ARM_L={(millimeter):[35,65,100]} as LengthBoundSpec;
export const ARM_W={(millimeter):[8,16,30]} as LengthBoundSpec;
export const ARM_T={(millimeter):[3,5,10]} as LengthBoundSpec;
annotation { "Feature Type Name" : "05_RobotSoccer Kicker Arm" }
export const robotSoccerKickerArm=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Length"} isLength(definition.l,ARM_L);
 annotation{"Name":"Width"} isLength(definition.w,ARM_W);
 annotation{"Name":"Thickness"} isLength(definition.t,ARM_T);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.t});
},{"l":65*millimeter,"w":16*millimeter,"t":5*millimeter});

// ===== 06_RobotSoccerSolenoidMount.fs =====
export const SOL_W={(millimeter):[15,28,50]} as LengthBoundSpec;
export const SOL_L={(millimeter):[20,45,80]} as LengthBoundSpec;
export const SOL_T={(millimeter):[2,3,6]} as LengthBoundSpec;
annotation { "Feature Type Name" : "06_RobotSoccer Solenoid Mount" }
export const robotSoccerSolenoidMount=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Width"} isLength(definition.w,SOL_W);
 annotation{"Name":"Length"} isLength(definition.l,SOL_L);
 annotation{"Name":"Thickness"} isLength(definition.t,SOL_T);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.t});
},{"w":28*millimeter,"l":45*millimeter,"t":3*millimeter});

// ===== 07_RobotSoccerTopCover.fs =====
export const COVER_W={(millimeter):[100,180,250]} as LengthBoundSpec;
export const COVER_L={(millimeter):[100,180,250]} as LengthBoundSpec;
export const COVER_T={(millimeter):[1.5,2.5,5]} as LengthBoundSpec;
annotation { "Feature Type Name" : "07_RobotSoccer Top Cover" }
export const robotSoccerTopCover=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Width"} isLength(definition.w,COVER_W);
 annotation{"Name":"Length"} isLength(definition.l,COVER_L);
 annotation{"Name":"Thickness"} isLength(definition.t,COVER_T);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.t});
},{"w":180*millimeter,"l":180*millimeter,"t":2.5*millimeter});

// ===== 08_RobotSoccerBallGuide.fs =====
export const GUIDE_L={(millimeter):[25,55,90]} as LengthBoundSpec;
export const GUIDE_W={(millimeter):[8,15,30]} as LengthBoundSpec;
export const GUIDE_H={(millimeter):[8,20,40]} as LengthBoundSpec;
annotation { "Feature Type Name" : "08_RobotSoccer Ball Guide" }
export const robotSoccerBallGuide=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Length"} isLength(definition.l,GUIDE_L);
 annotation{"Name":"Width"} isLength(definition.w,GUIDE_W);
 annotation{"Name":"Height"} isLength(definition.h,GUIDE_H);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.h});
},{"l":55*millimeter,"w":15*millimeter,"h":20*millimeter});

// ===== 09_RobotSoccerN20MotorMockup.fs =====
export const MOTOR_W={(millimeter):[8,12,25]} as LengthBoundSpec;
export const MOTOR_L={(millimeter):[20,36,60]} as LengthBoundSpec;
export const MOTOR_H={(millimeter):[8,10,25]} as LengthBoundSpec;
annotation { "Feature Type Name" : "09_RobotSoccer N20 Motor Mockup" }
export const robotSoccerN20MotorMockup=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Width"} isLength(definition.w,MOTOR_W);
 annotation{"Name":"Length"} isLength(definition.l,MOTOR_L);
 annotation{"Name":"Height"} isLength(definition.h,MOTOR_H);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skRectangle(s,"r",{"firstCorner":vector(-definition.w/2,-definition.l/2),"secondCorner":vector(definition.w/2,definition.l/2)});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.h});
},{"w":12*millimeter,"l":36*millimeter,"h":10*millimeter});

// ===== 10_RobotSoccerOmniWheelMockup.fs =====
export const WHEEL_D={(millimeter):[40,60,80]} as LengthBoundSpec;
export const WHEEL_W={(millimeter):[15,24,40]} as LengthBoundSpec;
annotation { "Feature Type Name" : "10_RobotSoccer Omni Wheel Mockup" }
export const robotSoccerOmniWheelMockup=defineFeature(function(context is Context,id is Id,definition is map)
precondition{
 annotation{"Name":"Diameter"} isLength(definition.d,WHEEL_D);
 annotation{"Name":"Width"} isLength(definition.w,WHEEL_W);
}{
 var s=newSketch(context,id+"s",{"sketchPlane":qCreatedBy(makeId("Top"),EntityType.FACE)});
 skCircle(s,"c",{"center":vector(0*millimeter,0*millimeter),"radius":definition.d/2});
 skSolve(s);
 extrude(context,id+"e",{"entities":qSketchRegion(id+"s"),"endBound":BoundingType.BLIND,"depth":definition.w});
},{"d":60*millimeter,"w":24*millimeter});

