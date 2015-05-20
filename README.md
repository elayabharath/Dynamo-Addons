![Build Status](http://dynamobim.com/app/badge.svg)

Dynamo-Addons
==================

Project to add useful add-ons that will stretch the capabilities of Dynamo library. For Dynamo project, please head out to https://github.com/DynamoDS/Dynamo

##Illustrator##
============
- Export all 2D Dynamo geometry into Illustrator or InkScape via SVG. This is currently a work in progress, please send your feedback to elayabharath@gmail.com or aparajit.pratap@autodesk.com / please log issues under this repository.

**Roadmap for Illustrator package**

1. Colour support SVG - allows the programmatic creation of colours for the exported geometry.  (This will require Dynamo to give out color information along with geometry, which it doesn't do at this point. This  will have to wait until Dynamo is ready)

2. Projected geometry views into illustrator - this would help in preparing artistic impressions directly from Dynamo geometry. Take an isometric view of the building --> smartly pick up all the visible edges and planes --> export this SVG --> view in Illustrator to add details like landscape

3. Add more support for Dynamo geometry - smart conversions for surfaces, solids, nonplanar curves. Handle illustrator groups.

4. Add native Illustrator geometry - this allows adding illustrator's appearances and styling like drop-shadow. Add more geometry features like stars, rounded corners to shapes, tangent finding, passing a photo to convert to vector.

5. Automate Illustrator without having to write javscript code in extend toolkit. Link for extend toolkit  https://creative.adobe.com/products/estk. 

0. Read SVG into Dynamo - this is already partially done. To clean up code and publish to package.

![alt tag](/Resources/screen.png)


**GPS track viewer**
- View your gps track from the gpx file, with the help of Google maps
Limitations: 1. Only the first track will be plotted 
             2. All the track segments will be joined as one.
             3. The track will be approximated to suit the http request requiements
WARNNG: Google maps allows only 25000 requests per day per application, please be considerate in number of requests you use :)
![alt tag](/Resources/gps.png)


**Grapher**
- Added hover over support on svg plot
- Ability to add pie chart with legend
- Ability to create scatter plot and histogram added
- Single series plot only supported

![alt tag](/Resources/pieChart.png)
![alt tag](/Resources/scatterPlot.png)
![alt tag](/Resources/histogramPlot.png)

![BuildStatus](http://dynamobim.com/app/display.php/?buildstatus=1&teststatus=1&coverage=1)
