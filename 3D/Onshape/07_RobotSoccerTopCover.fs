FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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