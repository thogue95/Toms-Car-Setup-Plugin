Disclaimer: I am posting the plugin and dashboards early because there has been significant interest in them.  They are a work in progress and may have issues.

Compiled Plugin Installation:

First check to make sure you do not have any plugins generating error messages by checking the Simhum System Log.

![Simhub clean log file](https://user-images.githubusercontent.com/8271391/141019524-b8d6e433-053d-4700-9b8e-3a64a30c6ac0.png)


Download the two files and move them to the Simhub directory.  When you start up Simhub, it will ask you if you want to install TomHogue.CarSetup Plugin. Click Yes.

Again check the log file to make sure there are no repeating error messages.

Verify that you can see the Car Setup plugin properities in Available Properties.

![Car Setup Properties 1](https://user-images.githubusercontent.com/8271391/141019606-25516977-4161-4e86-b89e-64ba69b81967.png)
![Car Setup Properties 2](https://user-images.githubusercontent.com/8271391/141019621-ffcaddb4-fedd-457d-9b55-292aff82498d.png)

Plugin Removal:

Go to the Simhub directory and remove the two files - TomHogue.CarSetupPlugin.dll and the TomHogue.CarSetupPlugin.pdb.
Restart Simhub and you should see the properties are no longer available.
