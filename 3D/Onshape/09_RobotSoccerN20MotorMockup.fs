FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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