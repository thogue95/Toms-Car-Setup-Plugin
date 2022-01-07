# Toms-Car-Setup-Plugin

Update 1-7-2022: Adding installation instructions for Simhub and plugins.

Update 11-20-2021: The lrShockDeflectionZero has been fixed but code not updated yet as I had realized that my calculations while correct, did not account for foces under speed like down force and how the tires are compressing under those loads.  Working on modeling and hoping to have an update soon.  This explains why my slow speed laps have accurate ride heights but full speed laps do not.  As a result, we will have Downforce numbers available in a dashboard when this is completed.

Update 11-19-2021: I have found that the lrShockDeflectionZero variable is at 0 when it should have a value.  Working on resolving it.


Plugin to expose data for car setup dashboards and associated dashboards

Important!!!  Read before you do anything.

This plugin exposes data from the Simhub Raw Data feed so that we can make some calculations on it that we use when we create racing setups.  

Currently, it is only supporting the SKModifieds, Tour Modifieds, NextGen and Trucks. 

There are two types of data it exposes.  Static (Off Track) and Dynamic (On Track).

The Static(Off Track) data is essentially what is available in the Garage for ride heights, weight distributions and rake.  You can easily just use a calculator to find these numbers, but the idea is that you can use a Simhub dashboard to be able to see it while in the car.  They are accurate and work perfectly.

The dynamic numbers are a WORK IN PROGRESS.  These numbers are derived mostly from the shock travel telemetry feeds and so while I am pretty confident in my calculations, there is a problem with the data when I compare them to data in the telemtry apps like Atlas and Motec.  

Translated: My dynamic numbers are not accurate yet so play with them knowing they are off a little.  Interesting part is that the numbers are accurate when the car is on the track and I take a slow lap but on fast laps they lose accuracy in fast laps.  Here is a pic of the Simhub data that is exposed. 


![Car Setup Properties 1](https://user-images.githubusercontent.com/8271391/141012617-e09c778d-3826-4d98-bc1d-1e6c324b2c49.png)
![Car Setup Properties 2](https://user-images.githubusercontent.com/8271391/141012629-167b2403-d644-4757-b079-51998c63ae57.png)

Here are a pic of my screen with the dashboards that I created with the new data feeds provided by the plugin.


![full screen next gen overlays](https://user-images.githubusercontent.com/8271391/141017841-1df8426d-fd1f-4b2f-b71c-b5b9d6be591e.png)


I have created directories for the source code, compiled plugins and the dashboards.  Remember, you can make your own dashboards using the new data feeds.

If you want to just take the easy way then go to the compiled plug in page and read the installation steps. It is super easy.  Then go to the dashboard pages and download the dashboards.  Instructions are there as well.

The source code files are there for anyone to do some programming and compile yourself.

Plugin Installation:

Before you install this plugin, go to your Simhub and verify that you are not generating errors by looking at the System Log as shown in the pic below.  I have found other plugins looking errors over and over such that it slows down your system. You will know you have a problem when you see a never ending repeat of the same message. Here is a pic of a clean log.

![Simhub clean log file](https://user-images.githubusercontent.com/8271391/141016072-58173586-2009-4b07-aad4-f7b96353dd56.png)



