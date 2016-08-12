# influxdb-csharp-console
A quick console app to run a few operations with InfluxDb

When run , it will:

1. list the databases in the InfluxDb endpoint
2. create a test database
3. send metrics to the test database once a second (as of this writing)

## Configuration
- change the app.config settings to your liking

## To run:

1. open in Visual Studio 2015 (other versions may also work, YMMV)
2. build solution (you may need to enable nuget package restore if it is not already)
3. run it
