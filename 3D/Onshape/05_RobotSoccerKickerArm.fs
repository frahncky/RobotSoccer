FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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