FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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