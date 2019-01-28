RaceTrackHeightmapGenerator 

TODO - THIS README!!!

Also, TODO a help button in the GUI with necessary info. 

Mention things like:

white borders of at least 1px. Track must always be at least 2px wide - I think. Place starting masks with one point on black and one on white, next to eachother, with the masks being the shortest distance from eachother, on corresponding sides of the track. Input image is a purely black and white image, but must NOT be a monochromatic bitmap as we use coloured pixels while processing - though can still use a mono bmp as a middle step to get your final full depth b/w bmp. etc)

For the gradient profile, mention threshold, ideally b/w bmp, no white edges, but make sure to keep some height at even the lowest point so you don't end up with a zero value on the geyscale (bad for blender extrude) etc...
