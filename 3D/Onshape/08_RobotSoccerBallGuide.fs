FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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