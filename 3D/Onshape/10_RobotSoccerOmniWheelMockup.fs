FeatureScript 1890;
import(path : "onshape/std/geometry.fs", version : "1890.0");
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