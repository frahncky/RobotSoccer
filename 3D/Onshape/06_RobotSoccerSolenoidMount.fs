FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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