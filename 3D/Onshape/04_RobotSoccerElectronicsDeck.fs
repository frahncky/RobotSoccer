FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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