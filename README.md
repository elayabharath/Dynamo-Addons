Dynamo-Addons
==================

Project to add useful add-ons that will stretch the capabilities of Dynamo library. For Dynamo project, please head out to https://github.com/DynamoDS/Dynamo

GPS track viewer
- View your gps track from the gpx file, with the help of Google maps
Limitations: 1. Only the first track will be plotted 
             2. All the track segments will be joined as one.
             3. The track will be approximated to suit the http request requiements
WARNNG: Google maps allows only 25000 requests per day per application, please be considerate in number of requests you use :)
![alt tag](/Resources/gps.png)


Grapher
- Added hover over support on svg plot
- Ability to add pie chart with legend
- Ability to create scatter plot and histogram added
- Single series plot only supported

![alt tag](/Resources/pieChart.png)
![alt tag](/Resources/scatterPlot.png)
![alt tag](/Resources/histogramPlot.png)


SVGPort update
- Currently supports points, lines, polygons, ellipses, circles.
- Ignores all Z values, the objects need to be planar in XY Plane
- Curve support coming soon

![alt tag](/Resources/screen.png)
